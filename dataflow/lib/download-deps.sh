if [ ! -d tmp ]; then
	mkdir tmp
fi

rm -rf tmp/*

wget http://ufpr.dl.sourceforge.net/sourceforge/nmock/nmock2-2.0.0.44-net-2.0.zip -O tmp/nmock.zip
cd tmp
unzip nmock.zip
cp bin/*.dll ../bin
cd ..
rm -rf tmp/*

