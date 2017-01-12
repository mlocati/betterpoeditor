VERSION = 1.2.0
INSTALL_PATH?=/usr/local
DOLLAR=$$

all:monobins

# Mono compiled binaries
monobins:../src/BetterPoEditor/bin/Release/BetterPoEditor.exe ../src/NetSpell.DictionaryBuild/bin/Release/NetSpell.DictionaryBuild.exe

../src/Jayrock.Json/bin/Release/Jayrock.Json.dll:
	@./build-project Jayrock.Json

../src/NetSpell.SpellChecker/bin/Release/NetSpell.SpellChecker.dll:
	@./build-project NetSpell.SpellChecker

../src/NetSpell.DictionaryBuild/bin/Release/NetSpell.DictionaryBuild.exe:../src/NetSpell.SpellChecker/bin/Release/NetSpell.SpellChecker.dll
	@./build-project NetSpell.DictionaryBuild

../src/TidyNet/bin/Release/TidyNet.dll:
	@./build-project TidyNet

../src/BetterPoEditor/bin/Release/BetterPoEditor.exe:../src/Jayrock.Json/bin/Release/Jayrock.Json.dll ../src/NetSpell.SpellChecker/bin/Release/NetSpell.SpellChecker.dll ../src/TidyNet/bin/Release/TidyNet.dll
	@./build-project BetterPoEditor

install:monobins
	@echo -n "Installing library files in ${INSTALL_PATH}/lib/betterpoeditor... "
	@if [ ! -d ${INSTALL_PATH}/lib/betterpoeditor ]; then mkdir ${INSTALL_PATH}/lib/betterpoeditor; fi
	@cp ./run-mono-exe ${INSTALL_PATH}/lib/betterpoeditor
	@cp ../src/Jayrock.Json/bin/Release/Jayrock.Json.dll ${INSTALL_PATH}/lib/betterpoeditor
	@cp ../src/NetSpell.SpellChecker/bin/Release/NetSpell.SpellChecker.dll ${INSTALL_PATH}/lib/betterpoeditor
	@cp ../src/NetSpell.DictionaryBuild/bin/Release/NetSpell.DictionaryBuild.exe ${INSTALL_PATH}/lib/betterpoeditor
	@cp ../src/TidyNet/bin/Release/TidyNet.dll ${INSTALL_PATH}/lib/betterpoeditor
	@cp ../src/BetterPoEditor/bin/Release/BetterPoEditor.exe ${INSTALL_PATH}/lib/betterpoeditor
	@cp --recursive ../src/BetterPoEditor/dictionaries ${INSTALL_PATH}/lib/betterpoeditor
	@cp --recursive ../contribs ${INSTALL_PATH}/lib/betterpoeditor
	@echo "done."
	@echo "Creating launch scripts in ${INSTALL_PATH}/bin:"
	@echo -n "\tbetterpoeditor... "
	@echo -n "#!/bin/sh\n\`dirname \\\`readlink -f " >${INSTALL_PATH}/bin/betterpoeditor
	@echo -n ${DOLLAR} >>${INSTALL_PATH}/bin/betterpoeditor
	@echo -n "0\\\`\`/../lib/betterpoeditor/run-mono-exe BetterPoEditor.exe \"" >>${INSTALL_PATH}/bin/betterpoeditor
	@echo -n ${DOLLAR} >>${INSTALL_PATH}/bin/betterpoeditor
	@echo "1\"" >>${INSTALL_PATH}/bin/betterpoeditor
	@chmod a+x ${INSTALL_PATH}/bin/betterpoeditor
	@echo "done."
	@echo -n "\tbetterpoeditor-dictionarybuild... "
	@echo -n "#!/bin/sh\n\`dirname \\\`readlink -f " >${INSTALL_PATH}/bin/betterpoeditor-dictionarybuild
	@echo -n ${DOLLAR} >>${INSTALL_PATH}/bin/betterpoeditor-dictionarybuild
	@echo "0\\\`\`/../lib/betterpoeditor/run-mono-exe NetSpell.DictionaryBuild.exe " >>${INSTALL_PATH}/bin/betterpoeditor-dictionarybuild
	@chmod a+x ${INSTALL_PATH}/bin/betterpoeditor-dictionarybuild
	@echo "done."

uninstall:
	@- $(RM) ${INSTALL_PATH}/bin/betterpoeditor-dictionarybuild
	@- $(RM) ${INSTALL_PATH}/bin/betterpoeditor
	@- $(RM) --recursive ${INSTALL_PATH}/lib/betterpoeditor

clean:
	@- $(RM) ./build.log
	@- $(RM) --recursive ../src/BetterPoEditor/bin
	@- $(RM) --recursive ../src/TidyNet/bin
	@- $(RM) --recursive ../src/NetSpell.DictionaryBuild/bin
	@- $(RM) --recursive ../src/NetSpell.SpellChecker/bin
	@- $(RM) --recursive ../src/Jayrock.Json/bin

distclean:clean

help:
	@echo "Available make options:"
	@echo "make help     : shows this message"
	@echo "make          : compiles everything"
	@echo "make install  : installs the compiled things into the system"
	@echo "make uninstall: removes the compiled things from the system"
	@echo "make clean    : removes compiled intermediary files (resets the source code"
	@echo "                to its initial state)"

.PHONY:all clean distclean monobins install uninstall help

