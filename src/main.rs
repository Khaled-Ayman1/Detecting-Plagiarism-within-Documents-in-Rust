use rayon::join;
use std::fs::File;
use std::io;
use std::io::BufRead;
use std::io::BufReader;
use std::time::Instant;

mod problem;

//best score
//average execution time (ms) = 159
//Max execution time (ms) = 446

fn read_file(file_path: &str) -> io::Result<Vec<String>> {
    let file = File::open(file_path)?;
    let reader = BufReader::new(file);

    let lines: Vec<String> = reader.lines().collect::<Result<_, _>>()?;

    Ok(lines)
}

fn run_tests(file_path: &str) {
    let lines = match read_file(file_path) {
        Ok(lines) => lines,
        Err(err) => {
            eprintln!("Error reading file: {}", err);
            return;
        }
    };

    // Extract the number of test cases
    let n_cases = match lines.first().and_then(|s| s.parse::<usize>().ok()) {
        Some(val) => val,
        None => {
            eprintln!("Invalid number of test cases");
            return;
        }
    };

    let mut iter = lines.iter().skip(1); // Skip the first line (number of test cases)

    let mut total_time: f64 = 0.0;
    let mut max_time: f64 = 0.0;
    for i in 0..n_cases {
        // Process each test case
        if let Some(doc1_path) = iter.next() {
            if let Some(doc2_path) = iter.next() {
                if let Some(expected_result) = iter.next() {
                    // Process the test case
                    let doc1_path = doc1_path.trim();
                    let doc2_path = doc2_path.trim();
                    let expected_result = expected_result.trim();

                    // Implement your test case logic here
                    print!("Case {}: ", (i + 1) as i32);

                    let start_time = Instant::now();
                    let actual_result = problem::calculate_distance(doc1_path, doc2_path);
                    let elapsed_time = start_time.elapsed();
                    if elapsed_time.as_millis() as f64 > max_time {
                        max_time = elapsed_time.as_millis() as f64;
                    }
                    total_time += elapsed_time.as_millis() as f64;
                    if f64::round(expected_result.parse::<f64>().unwrap())
                        != f64::round(actual_result)
                    {
                        println!("wrong answer!");
                        println!("your answer = {}", actual_result);
                        println!("correct answer = {}", expected_result);
                    }
                    if elapsed_time.as_millis() > 20000 {
                        eprintln!("time limit exceed: case #{}", (i + 1) as i32);
                        return;
                    }
                    println!("completely succeed");
                } else {
                    eprintln!("Incomplete test case: missing expected result");
                    return;
                }
            } else {
                eprintln!("Incomplete test case: missing document 2 path");
                return;
            }
        } else {
            eprintln!("Incomplete test case: missing document 1 path");
            return;
        }
    }
    println!("test is finished");

    println!(
        "average execution time (ms) = {}",
        f64::round(total_time / (n_cases as f64))
    );
    println!("Max execution time (ms) = {}", f64::round(max_time));
}

fn main() {
    println!("Document Distance:\n[1] Sample test cases\n[2] Complete testing\n");
    println!("Enter your choice [1-2]: ");

    let mut choice = String::new();
    io::stdin()
        .read_line(&mut choice)
        .expect("Failed to read line");

    match choice.trim() {
        "1" => run_tests("C:\\Users\\user\\Desktop\\Home\\Study\\University\\GP\\Rust\\Detecting-Plagiarism-within-Documents-in-Rust\\tests\\Sample.txt"),
        "2" => run_tests("C:\\Users\\user\\Desktop\\Home\\Study\\University\\GP\\Rust\\Detecting-Plagiarism-within-Documents-in-Rust\\tests\\complete.txt"),
        _ => println!("Invalid choice!"),
    }
}
