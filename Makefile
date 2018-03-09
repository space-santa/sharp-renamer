.PHONY : run
run:
	mono srenamer.exe

.PHONY : br
br : build run

.PHONY : build
build:
	fsharpc srenamer.fs