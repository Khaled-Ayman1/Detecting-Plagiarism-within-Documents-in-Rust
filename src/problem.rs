use rayon::join;

use std::collections::hash_map::DefaultHasher;
use std::f64::consts::PI;
use std::hash::{Hash, Hasher};
use std::{collections::HashMap, fs};

pub fn calculate_distance(document1: &str, document2: &str) -> f64 {
    let (mut d0, mut d1, mut d2): (f64, f64, f64) = (0.0f64, 0.0f64, 0.0f64);

    let (doc_string1, doc_string2) = join(
        || fs::read_to_string(document1).expect("Error"),
        || fs::read_to_string(document2).expect("Error"),
    );

    let (doc_hash1, doc_hash2) = join(|| split_string(doc_string1), || split_string(doc_string2));

    join(
        || {
            if doc_hash1.len() > doc_hash2.len() {
                for s in doc_hash2.keys() {
                    if doc_hash1.contains_key(s) {
                        if let (Some(&v1), Some(&v2)) = (doc_hash1.get(s), doc_hash2.get(s)) {
                            if let Some(product) = i32::checked_mul(v1, v2) {
                                d0 += product as f64;
                            }
                        }
                    }
                }
            } else {
                for s in doc_hash1.keys() {
                    if doc_hash2.contains_key(s) {
                        if let (Some(&v1), Some(&v2)) = (doc_hash1.get(s), doc_hash2.get(s)) {
                            if let Some(product) = i32::checked_mul(v1, v2) {
                                d0 += product as f64;
                            }
                        }
                    }
                }
            }
        },
        || {
            join(
                || {
                    for i in doc_hash1.values() {
                        d1 += i.pow(2) as f64;
                    }
                },
                || {
                    for i in doc_hash2.values() {
                        d2 += i.pow(2) as f64;
                    }
                },
            );
        },
    );
    calculate_angle(d0, d1, d2)
}

fn split_string(doc_string: String) -> HashMap<u64, i32> {
    let mut doc_hash: HashMap<u64, i32> = HashMap::new();
    let characters: Vec<char> = doc_string.chars().collect();
    let mut start = 0;
    let mut str: String;

    for i in 0..characters.len() {
        if !characters[i].is_alphanumeric() {
            if start < i {
                str = characters[start..i - start].iter().collect();
                let hash_code = calculate_hash(&str);
                if doc_hash.contains_key(&hash_code) {
                    doc_hash.entry(hash_code).and_modify(|e| *e += 1);
                } else {
                    doc_hash.insert(hash_code, 1);
                }
            }
            start = i + 1;
        }
    }
    if start < characters.len() {
        str = characters[start..characters.len() - start].iter().collect();
        let hash_code = calculate_hash(&str);
        if doc_hash.contains_key(&hash_code) {
            doc_hash.entry(hash_code).and_modify(|e| *e += 1);
        } else {
            doc_hash.insert(hash_code, 1);
        }
    }
    doc_hash
}

fn calculate_hash(t: &str) -> u64 {
    let mut hasher = DefaultHasher::new();
    t.hash(&mut hasher);
    hasher.finish()
}

fn calculate_angle(d0: f64, d1: f64, d2: f64) -> f64 {
    let cosine_angle = d0 / (f64::sqrt(d1 * d2));
    let angle_radians = cosine_angle.acos();
    let angle_degrees = angle_radians * (180.0 / PI);
    angle_degrees
}
