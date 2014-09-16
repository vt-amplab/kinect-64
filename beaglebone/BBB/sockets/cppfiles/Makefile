#*********************#
#* Sockets Makefile  *#
#* - Jacob Yacovelli *#
#* - 6/25/2014       *#
#*********************#

#	File structure
#	/
#		Makefile
#		includes/
#			ClientSock.h
#			ClientSock.cpp
#			ServerSock.h
#			ServerSock.cpp
#		tests/
#			clientLibTest.cpp
#			serverLibTest.cpp

CC := g++
DEBUG := 
#CFLAGS = -march=armv7-a -mtune=cortex-a8 -mfpu=neon -mfloat-abi=softfp -ffast-math -O3 -Wall $(DEBUG)
CFLAGS = -c -O3 -Wall $(DEBUG)
LDFLAGS = -Wall $(DEBUG)

INCDIR := includes/
LIBDIR := libs/
EXEDIR := bin/
TSTDIR := tests/
INCLUDES = $(shell ls $(INCDIR)*.cpp)
OBJECTS = $(subst $(INCDIR),$(LIBDIR),$(INCLUDES:.cpp=.o))
DEPENDANTS = $(shell ls $(INCDIR)*.h)
TESTS = $(shell ls $(TSTDIR)*.cpp)
OUTPUT = $(subst $(TSTDIR),,$(TESTS:.cpp=))
ARCHIVE = lib$(OBJECTS:.o=.a)

test: $(TESTS)

$(TESTS): $(OBJECTS)
	$(eval LIBOBJ = $(subst .o,,$(subst $(LIBDIR),,$(word 1, $(OBJECTS)))))
	$(CC) $(LDFLAGS) $@ -I$(INCDIR) -L$(LIBDIR) -l$(LIBOBJ) -o $(EXEDIR)$(subst $(TSTDIR),,$(subst .cpp,,$@))
	$(eval TMP = $(subst $<,,$(OBJECTS)))
	$(eval OBJECTS = $(TMP))

$(LIBDIR)%.o: $(INCDIR)%.cpp mkdir
	$(CC) $(CFLAGS) -I./$(INCDIR) $< -o $@
	ar rcs $(LIBDIR)lib$(subst .o,.a,$(subst $(LIBDIR),,$@)) $@

mkdir:
	$(shell mkdir $(LIBDIR) $(EXEDIR))

.PHONY: clean
clean:
	rm -rf $(LIBDIR) $(EXEDIR)