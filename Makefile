all: test

compile:
	gmcs @NHandlebars.rsp

test:
	gmcs -pkg:nunit /define:NUNIT @NHandlebars.rsp
	nunit-console NHandlebars.dll
