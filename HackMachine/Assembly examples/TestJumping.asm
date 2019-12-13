@0
M=0

//test JMP
@1
D=A
@JMP1
D;JMP
@0
M=M+1
(JMP1)


//test JEQ
@0
D=A
@JMP2
D;JEQ
@0
M=M+1
(JMP2)


//test JLT
@1
D=0
D=D-A
@JMP3
D;JLT
@0
M=M+1
(JMP3)


//test JGT
@1
D=A
@JMP4
D;JGT
@0
M=M+1
(JMP4)

