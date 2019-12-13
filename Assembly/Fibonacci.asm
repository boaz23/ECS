// Decalre and initalize variables (copying arguments to variables too)

// array = M[1]
// n = M[2]
@1
D=M
@array
M=D
@1
M=0

@2
D=M
@n
M=D
@2
M=0

// before loop initialization
// R0 = 0
// array[R0] = 1 // M[M[array] + M[R0]] = 1
// R1 = 1
// array[R1] = 1 // M[M[array] + M[R1]] = 1
// R2 = 2
@R0
M=0

D=M
@array
A=M+D
M=1

@R1
M=1

D=M
@array
A=M+D
M=1

@2
D=A
@R2
M=D

(WHILE)
	// loop condition check
	// R2 < n
	@R2
	D=M
	@n
	D=M-D
	@END_WHILE
	D;JLE

	// loop body
	// M[M[array] + M[R2]] = M[M[array] + M[R0]] + M[M[array] + M[R1]]
	
	// M[R3] = M[M[array] + M[R0]]
	@R0
	D=M
	@array
	A=M+D
	D=M
	@R3
	M=D

	// M[R4] = M[M[array] + M[R1]]
	@R1
	D=M
	@array
	A=M+D
	D=M
	@R4
	M=D

	// M[R3] = M[R3] + M[R4]
	@R4
	D=M
	@R3
	M=M+D

	// M[R4] = M[array] + M[R2]
	@R2
	D=M
	@array
	D=D+M
	@R4
	M=D

	// M[M[R4]] = M[R3]
	@R3
	D=M
	@R4
	A=M
	M=D

	// loop step increment
	// R0 = R0 + 1
	// R1 = R1 + 1
	// R2 = R2 + 1
	@R0
	M=M+1
	@R1
	M=M+1
	@R2
	M=M+1
	@WHILE
	0;JMP
(END_WHILE)

// Empty endless loop
(END)
@END
0;JMP