#![warn(clippy::all)]
#![allow(clippy::upper_case_acronyms)]
#![doc(html_favicon_url = "https://huggingface.co/favicon.ico")]
#![doc(html_logo_url = "https://huggingface.co/landing/assets/huggingface_logo.svg")]

//! The core of `tokenizers`, written in Rust.
//! Provides an implementation of today's most used tokenizers, with a focus on performance and
//! versatility.
//!
//! # What is a Tokenizer
//!
//! A Tokenizer works as a pipeline, it processes some raw text as input and outputs an `Encoding`.
//! The various steps of the pipeline are:
//!
//! 1. The `Normalizer`: in charge of normalizing the text. Common examples of normalization are
//!    the [unicode normalization standards](https://unicode.org/reports/tr15/#Norm_Forms), such as `NFD` or `NFKC`.
//!    More details about how to use the `Normalizers` are available on the
//!    [Hugging Face blog](https://huggingface.co/docs/tokenizers/components#normalizers)
//! 2. The `PreTokenizer`: in charge of creating initial words splits in the text. The most common way of
//!    splitting text is simply on whitespace.
//! 3. The `Model`: in charge of doing the actual tokenization. An example of a `Model` would be
//!    `BPE` or `WordPiece`.
//! 4. The `PostProcessor`: in charge of post-processing the `Encoding` to add anything relevant
//!    that, for example, a language model would need, such as special tokens.
//!
//! ## Loading a pretrained tokenizer from the Hub
//! ```
//! use tokenizers::tokenizer::{Result, Tokenizer};
//!
//! fn main() -> Result<()> {
//!     # #[cfg(feature = "http")]
//!     # {
//!         let tokenizer = Tokenizer::from_pretrained("bert-base-cased", None)?;
//!
//!         let encoding = tokenizer.encode("Hey there!", false)?;
//!         println!("{:?}", encoding.get_tokens());
//!     # }
//!     Ok(())
//! }
//! ```
//!
//! ## Deserialization and tokenization example
//!
//! ```no_run
//! use tokenizers::tokenizer::{Result, Tokenizer, EncodeInput};
//! use tokenizers::models::bpe::BPE;
//!
//! fn main() -> Result<()> {
//!     let bpe_builder = BPE::from_file("./path/to/vocab.json", "./path/to/merges.txt");
//!     let bpe = bpe_builder
//!         .dropout(0.1)
//!         .unk_token("[UNK]".into())
//!         .build()?;
//!
//!     let mut tokenizer = Tokenizer::new(bpe);
//!
//!     let encoding = tokenizer.encode("Hey there!", false)?;
//!     println!("{:?}", encoding.get_tokens());
//!
//!     Ok(())
//! }
//! ```
//!
//! ## Training and serialization example
//!
//! ```no_run
//! use tokenizers::decoders::DecoderWrapper;
//! use tokenizers::models::bpe::{BpeTrainerBuilder, BPE};
//! use tokenizers::normalizers::{strip::Strip, unicode::NFC, utils::Sequence, NormalizerWrapper};
//! use tokenizers::pre_tokenizers::byte_level::ByteLevel;
//! use tokenizers::pre_tokenizers::PreTokenizerWrapper;
//! use tokenizers::processors::PostProcessorWrapper;
//! use tokenizers::{AddedToken, Model, Result, TokenizerBuilder};
//!
//! use std::path::Path;
//!
//! fn main() -> Result<()> {
//!     let vocab_size: usize = 100;
//!
//!     let mut trainer = BpeTrainerBuilder::new()
//!         .show_progress(true)
//!         .vocab_size(vocab_size)
//!         .min_frequency(0)
//!         .special_tokens(vec![
//!             AddedToken::from(String::from("<s>"), true),
//!             AddedToken::from(String::from("<pad>"), true),
//!             AddedToken::from(String::from("</s>"), true),
//!             AddedToken::from(String::from("<unk>"), true),
//!             AddedToken::from(String::from("<mask>"), true),
//!         ])
//!         .build();
//!
//!     let mut tokenizer = TokenizerBuilder::new()
//!         .with_model(BPE::default())
//!         .with_normalizer(Some(Sequence::new(vec![
//!             Strip::new(true, true).into(),
//!             NFC.into(),
//!         ])))
//!         .with_pre_tokenizer(Some(ByteLevel::default()))
//!         .with_post_processor(Some(ByteLevel::default()))
//!         .with_decoder(Some(ByteLevel::default()))
//!         .build()?;
//!
//!     let pretty = false;
//!     tokenizer
//!         .train_from_files(
//!             &mut trainer,
//!             vec!["path/to/vocab.txt".to_string()],
//!         )?
//!         .save("tokenizer.json", pretty)?;
//!
//!     Ok(())
//! }
//! ```
//!
//! # Additional information
//!
//! - tokenizers is designed to leverage CPU parallelism when possible. The level of parallelism is determined
//! by the total number of core/threads your CPU provides but this can be tuned by setting the `RAYON_RS_NUM_THREADS`
//! environment variable. As an example setting `RAYON_RS_NUM_THREADS=4` will allocate a maximum of 4 threads.
//! **_Please note this behavior may evolve in the future_**
//!
//! # Features
//! **progressbar**: The progress bar visualization is enabled by default. It might be disabled if
//!   compilation for certain targets is not supported by the [termios](https://crates.io/crates/termios)
//!   dependency of the [indicatif](https://crates.io/crates/indicatif) progress bar.

#[macro_use]
extern crate log;
#[macro_use]
extern crate lazy_static;

#[macro_use]
extern crate derive_builder;

#[macro_use]
pub mod utils;
pub mod decoders;
pub mod models;
pub mod normalizers;
pub mod pre_tokenizers;
pub mod processors;
pub mod tokenizer;

//use std::sync::Arc;

use serde::Serialize;
// Re-export from tokenizer
pub use tokenizer::*;

use libc::c_char;
use std::{ffi::CStr, path::Path};

// Re-export also parallelism utils
pub use utils::parallelism;

//static gb_tokenizer:Tokenizer = Tokenizer::from_pretrained("dccuchile/bert-base-spanish-wwm-cased", None).unwrap();
//Tokenizer::from_pretrained("dccuchile/bert-base-spanish-wwm-cased", None)

lazy_static! {
    static ref GLOBAL_TOKENIZER: Tokenizer =
        Tokenizer::from_pretrained("dccuchile/bert-base-spanish-wwm-cased", None).unwrap();
}

// Re-export for from_pretrained
#[cfg(feature = "http")]
pub use utils::from_pretrained::FromPretrainedParameters;

#[no_mangle]
pub extern "C" fn create_tokenizer(tokenizer_path: *const c_char) -> *mut Tokenizer {
    let tokenizer_name: String = unsafe {
        assert!(!tokenizer_path.is_null());
        CStr::from_ptr(tokenizer_path)
            .to_str()
            .expect("Can not read string argument.")
            .trim()
            .to_string()
    };
    let tokenizer: Tokenizer = Tokenizer::from_pretrained(tokenizer_name, None).unwrap();
    let x = Box::new(tokenizer);
    let p = Box::into_raw(x);
    return p;
}

#[no_mangle]
pub extern "C" fn create_tokenizer_local(tokenizer_path: *const c_char) -> *mut Tokenizer {
    let tokenizer_name: String = unsafe {
        assert!(!tokenizer_path.is_null());
        CStr::from_ptr(tokenizer_path)
            .to_str()
            .expect("Can not read string argument.")
            .trim()
            .to_string()
    };
    let root = Path::new(&tokenizer_name);
    let tokenizer: Tokenizer = Tokenizer::from_file(root).unwrap();

    let x = Box::new(tokenizer);
    let p = Box::into_raw(x);
    return p;
}

#[no_mangle]
pub extern "C" fn add_numbers(number1: i32, number2: i32) -> i32 {
    return number1 + number2;
}

#[no_mangle]
pub extern "C" fn print_string(text_pointer: *mut c_char) -> *mut c_char {
    unsafe {
        let text: String = CStr::from_ptr(text_pointer)
            .to_str()
            .expect("Can not read string argument.")
            .trim()
            .to_string();
        println!("{}", text);
        let ss: *mut c_char = text.as_ptr() as *mut c_char;
        return ss;
    }
}

// En Rust
#[repr(C)]
pub struct RustStringArray {
    pub len: usize,
    pub data: *const *const libc::c_char,
}


#[derive(Serialize)]
#[repr(C)]
pub struct RustArray {
    pub tokens: Vec<String>,
    pub ids: Vec<u32>,
    pub mask: Vec<u32>,
}

#[no_mangle]
pub extern "C" fn encode(text: *const c_char) -> RustStringArray {
    let c_str: &CStr = unsafe {
        assert!(!text.is_null());

        CStr::from_ptr(text)
    };

    let r_str: &str = c_str.to_str().unwrap();
    println!("{:?}", r_str);

    let tokenizer: Tokenizer =
        Tokenizer::from_pretrained("dccuchile/bert-base-spanish-wwm-cased", None).unwrap();
    let encoding: Encoding = tokenizer.encode(r_str, false).unwrap();
    println!("{:?}", encoding.get_tokens());
    let vec: Vec<String> = encoding.get_tokens().to_vec();

    let strings: Vec<_> = vec.iter().map(|s| s.as_ptr() as *const _).collect();
    let array: RustStringArray = RustStringArray {
        len: vec.len(),
        data: strings.as_ptr(),
    };
    return array;
}

#[no_mangle]
pub extern "C" fn encode_v3(text_pointer: *const c_char) -> *mut c_char {
    let mut text: String = unsafe {
        assert!(!text_pointer.is_null());
        CStr::from_ptr(text_pointer)
            .to_str()
            .expect("Can not read string argument.")
            .trim()
            .to_string()
    };
    //let mut texto = text.clone().to_string();
    println!("text: {}", text);
    let tokenizer: Tokenizer = GLOBAL_TOKENIZER.clone();
    let encoding: Encoding = tokenizer.encode(text, false).unwrap();
    let vec: Vec<String> = encoding.get_tokens().to_vec();
    let json: String = serde_json::to_string(&vec).unwrap();
    text = json.clone().to_string();
    println!("Json {:?}", text);
    let ss: *mut c_char = text.as_ptr() as *mut c_char;
    return ss;
}

#[no_mangle]
pub extern "C" fn encode_v4(
    tokenizerptr: *mut Tokenizer,
    text_pointer: *const c_char,
) -> *mut c_char {
    let mut text: String = unsafe {
        assert!(!text_pointer.is_null());
        CStr::from_ptr(text_pointer)
            .to_str()
            .expect("Can not read string argument.")
            .trim()
            .to_string()
    };
    println!("text: {}", text);

    //Clone the pointer
    let new_tokenizer: *mut Tokenizer = tokenizerptr.clone();
    let tokenizer: &Tokenizer = unsafe { &*new_tokenizer };

    let encoding: Encoding = tokenizer.encode(text, false).unwrap();

    let vec: Vec<String> = encoding.get_tokens().to_vec();

    let json: String = serde_json::to_string(&vec).unwrap();
    text = json.clone().to_string();
    println!("RUST Json {:?}", text);
    let ss: *mut c_char = text.as_ptr() as *mut c_char;
    return ss;
}

#[no_mangle]
pub extern "C" fn encode_v5(
    tokenizerptr: *mut Tokenizer,
    text_pointer: *const c_char,
) -> *mut c_char {
    let mut text: String = unsafe {
        assert!(!text_pointer.is_null());
        CStr::from_ptr(text_pointer)
            .to_str()
            .expect("Can not read string argument.")
            .trim()
            .to_string()
    };
    println!("text: {}", text);

    //Clone the pointer
    let new_tokenizer: *mut Tokenizer = tokenizerptr.clone();
    let tokenizer: &Tokenizer = unsafe { &*new_tokenizer };

    let encoding: Encoding = tokenizer.encode(text, false).unwrap();

    let vec_tokens: Vec<String> = encoding.get_tokens().to_vec();
    let vec_ids: Vec<u32> = encoding.get_ids().to_vec();
    let mask: Vec<u32> = encoding.get_attention_mask().to_vec();
    let struct_tokens: RustArray = RustArray {
        tokens: vec_tokens,
        ids: vec_ids,
        mask: mask,
    };

    let json = serde_json::to_string(&struct_tokens).unwrap();
    text = json.clone().to_string();
    println!("RUST Json {:?}", text);
    let ss: *mut c_char = text.as_ptr() as *mut c_char;
    return ss;
}