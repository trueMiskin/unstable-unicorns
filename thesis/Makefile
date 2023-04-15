NAME=thesis
ABSTRACT=abstract
LATEXMKOPTS=-pdf#enable for forcing pdflatex
LATEXMK=latexmk $(LATEXMKOPTS)
PANDOCOPTS=--top-level-division=chapter --lua-filter citation.lua #--bibliography=refs.bib  --citeproc

BUILD_DIR := ./build
CHAPTERS=1-introduction 2-game-description 3-related-work 4-game-modification

%.tex: %.md build_directory
	pandoc $(PANDOCOPTS) -f markdown -t latex -o "$(BUILD_DIR)/$@" $<

all: $(addsuffix .tex, $(CHAPTERS)) build_directory
	for x in $(CHAPTERS); do echo "\input{$$x}"; done > $(BUILD_DIR)/content.tex
	rsync -av ./latex_template/* $(BUILD_DIR)
	rsync -av refs.bib $(BUILD_DIR)
	
	cd $(BUILD_DIR); \
		$(LATEXMK) $(NAME); \
		$(LATEXMK) $(ABSTRACT)-cz; \
		$(LATEXMK) $(ABSTRACT)-en

1-introduction.tex: 1-introduction.md build_directory
	pandoc $(PANDOCOPTS) --lua-filter intro-outro.lua -f markdown -t latex -o "$(BUILD_DIR)/$@" $<

build_directory:
	mkdir -p $(BUILD_DIR)

clean:
	rm -rf $(BUILD_DIR)
