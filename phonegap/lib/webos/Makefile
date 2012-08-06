#
# Licensed to the Apache Software Foundation (ASF) under one
# or more contributor license agreements.  See the NOTICE file
# distributed with this work for additional information
# regarding copyright ownership.  The ASF licenses this file
# to you under the Apache License, Version 2.0 (the
# "License"); you may not use this file except in compliance
# with the License.  You may obtain a copy of the License at
#
#   http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing,
# software distributed under the License is distributed on an
# "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
# KIND, either express or implied.  See the License for the
# specific language governing permissions and limitations
# under the License.
#

GREP = /usr/bin/grep
CUT = /usr/bin/cut
SHELL = /bin/sh
CHMOD = chmod
CP = cp
MV = mv
NOOP = $(SHELL) -c true
RM_F = rm -f
RM_RF = rm -rf
TEST_F = test -f
TOUCH = touch
UMASK_NULL = umask 0
DEV_NULL = > /dev/null 2>&1
MKPATH = mkdir -p
CAT = cat
MAKE = make
OPEN = open
ECHO = echo
ECHO_N = echo -n
JAVA = java
PGVERSION = 2.0.0
UNAME := $(shell uname)

NAME = `$(CAT) framework/appinfo.json | $(GREP) '"id"' | $(CUT) -d \" -f 4`
VERSION = `$(CAT) framework/appinfo.json | $(GREP) '"version"' | $(CUT) -d \" -f 4`

all :: js copy_js package deploy run

custom :: js copy_js package deploy

clean :: clean_libs

clean_libs:
	-$(RM_RF) lib

package:
ifeq ($(UNAME), Linux)
	palm-package framework/
else
ifeq ($(UNAME), Darwin)
# mac OSX
	palm-package framework/
else
# assume windows OS
	palm-package.bat framework/
endif
endif


deploy:
ifeq ($(UNAME), Linux)
	palm-install $(NAME)_$(VERSION)_all.ipk
else
ifeq ($(UNAME), Darwin)
# mac OSX
	palm-install $(NAME)_$(VERSION)_all.ipk
else
# assume windows OS
	palm-install.bat $(NAME)_$(VERSION)_all.ipk
endif
endif

run:
ifeq ($(UNAME), Linux)
	palm-launch $(NAME)
else
ifeq ($(UNAME), Darwin)
# mac OSX
	palm-launch $(NAME)
else
# assume windows OS
	palm-launch.bat $(NAME)
endif
endif

copy_js:
	cp lib/cordova.js framework/cordova-$(PGVERSION).js

js: lib/cordova.js

lib/cordova.js: js/cordova-core.js js/acceleration.js js/accelerometer.js js/application.js js/audio.js js/camera.js js/compass.js js/contacts.js js/debugconsole.js js/device.js js/file.js js/geolocation.js js/map.js js/mojo.js js/mouse.js js/network.js js/notification.js js/orientation.js js/position.js js/service.js js/sms.js js/telephony.js js/window.js js/windowproperties.js lib/thumbs.0.5.2.js
	$(MKPATH) lib
	$(RM_F) $@
	$(CAT) js/cordova-core.js >> $@
	$(CAT) js/acceleration.js >> $@
	$(CAT) js/accelerometer.js >> $@
	$(CAT) js/application.js >> $@
	$(CAT) js/audio.js >> $@
	$(CAT) js/camera.js >> $@
	$(CAT) js/contacts.js >> $@
	$(CAT) js/compass.js >> $@
	$(CAT) js/debugconsole.js >> $@
	$(CAT) js/device.js >> $@
	$(CAT) js/file.js >> $@
	$(CAT) js/geolocation.js >> $@
	$(CAT) js/map.js >> $@
	$(CAT) js/mojo.js >> $@
	$(CAT) js/mouse.js >> $@
	$(CAT) js/network.js >> $@
	$(CAT) js/notification.js >> $@
	$(CAT) js/orientation.js >> $@
	$(CAT) js/position.js >> $@
	$(CAT) js/service.js >> $@
	$(CAT) js/sms.js >> $@
	$(CAT) js/telephony.js >> $@
	$(CAT) js/window.js >> $@
	$(CAT) js/windowproperties.js >> $@
	$(CAT) lib/thumbs.0.5.2.js >> $@