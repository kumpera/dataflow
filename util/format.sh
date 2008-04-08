#!/bin/sh
#
# Copyright © 2008 The Dataflow Team
#
# See AUTHORS and LICENSE for details.

if [ `which astyle` ]; then 
  cd .. && find . -name *.cs | xargs astyle --mode=cs -U -l -p --style=kr
  exit $?

else
  echo 'Artistic Style not found: download and install from astyle.sf.net'
  exit 1

fi
