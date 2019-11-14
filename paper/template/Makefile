SRC = arnoldz3-Factor
all:
	pdflatex $(SRC)
	bibtex $(SRC)
	pdflatex $(SRC)
	pdflatex $(SRC)

clean:
	rm *.pdf $(SRC).log $(SRC).out $(SRC).aux $(SRC).bbl $(SRC).blg comment.cut
