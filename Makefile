.PHONY : run
run:
	mono srenamer.exe $(filter-out $@,$(MAKECMDGOALS))

.PHONY : br
br : build run

.PHONY : build
build:
	fsharpc srenamer.fs