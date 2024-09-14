# Calculate Distance

Given: two documents D1 & D2

Calculate distance between them d(D1, D2)? 

CASE INSENSITIVE


# Defenitions
Word = sequence of alpha-numeric characters ONLY

Document = sequence of words (ignore space, punctuation, …etc.) 

Treat all upper-case letters as if they are lower-case, so “Cat" & “cat" are same

Word end at a non-alphanumeric char, so "can't" contains 2 words: "can" & "t"



# Idea

Idea: define distance in terms of shared words. 

Think of document D as a vector

D[w] = # occurrences of word w


# Example
Example: 	D1 = “the cat”	D2 = “the dog” 

D1[“the”] = 1, D1[“cat”] = 1

D2[“the”] = 1, D2[“dog”] = 1

d(D1,D2) = angle between 2 vectors. 

0◦: identical, 90◦: no common words


![image](https://github.com/y0sif/calculate-distance/assets/61329766/a1e5239b-2bc8-482c-8c63-d97a7d79b47f)

![image](https://github.com/y0sif/calculate-distance/assets/61329766/5c931a1b-4b82-4190-b1b8-92e597312b32)

![image](https://github.com/y0sif/calculate-distance/assets/61329766/a885ad32-aae2-4759-8256-2ceefe31f7b3)


# Algorithm
Split each document into words 

Count word frequencies (document vectors) 

NOTE: Max Freq. Value BOUNDED BY 100,000

Compute distance

# Where to Start?
all code to be written inside problem.rs

![image](https://github.com/y0sif/calculate-distance/assets/61329766/7f71bf7c-4f27-470c-b00c-e312627b0dbe)


# Best Score to beat
average execution time (ms) = 118

Max execution time (ms) = 235

# How to run tests?
use the following command

```
cargo run
```

for release test use the following command

```
cargo run --release
```

