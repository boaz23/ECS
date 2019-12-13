// Copy arguments to "labels" and declare variables

// k = 0
@k
M=0

// n = M[1]
@1
D=M
@n
M=D
@1
M=0

// m = M[2]
@2
D=M
@m
M=D
@2
M=0

// Before loop initialization
@R0
M=0
(WHILE)
// loop condition check
@R0
D=M
@m
D=D+M
@R1
M=D
@n
D=M-D
@END_WHILE
D;JLT

// loop body
@R1
D=M
@R0
M=D
@k
M=M+1
@WHILE
0;JMP
(END_WHILE)

// copy variables to output
@k
D=M
@0
M=D

(END)
@END
0;JMP