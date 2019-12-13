// Decalre and initalize variables (copying arguments to variables too)

// n = M[1]
// m = M[2]
// p = 1
@1
D=M
@n
M=D
@1
M=0

@2
D=M
@m
M=D
@2
M=0

@p
M=1

// before outer loop initialization
// R0 = 0
@R0
M=0
(WHILE_OUTER)
	// outer loop condition check
	// R0 < m
	@R0
	D=M
	@m
	D=M-D
	@END_WHILE_OUTER
	D;JLE

	// outer loop body
	// before inner loop initialization
	// R1 = 0
	// R2 = p
	// R3 = 0
	@R1
	M=0

	@p
	D=M
	@R2
	M=D

	@R3
	M=0
	(WHILE_INNER)
		// inner loop condition check
		// R1 < n
		@R1
		D=M
		@n
		D=M-D
		@END_WHILE_INNER
		D;JLE

		// inner loop body
		// R3 = R3 + R2
		@R2
		D=M
		@R3
		M=M+D

		// inner loop step increment
		// R1 = R1 + 1
		@R1
		M=M+1
		@WHILE_INNER
		0;JMP
	(END_WHILE_INNER)

	// p = R3
	@R3
	D=M
	@p
	M=D
	
	// outer loop step increment
	// R0 = R0 + 1
	@R0
	M=M+1
	@WHILE_OUTER
	0;JMP
(END_WHILE_OUTER)

// Copy variables to output
@p
D=M
@0
M=D

// Empty endless loop
(END)
@END
0;JMP