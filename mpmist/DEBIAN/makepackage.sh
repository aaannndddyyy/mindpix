cd ..
rm -r temp
mkdir temp
mkdir temp/DEBIAN
mkdir temp/usr
mkdir temp/usr/bin
cp DEBIAN/control temp/DEBIAN
cp DEBIAN/install temp/DEBIAN
cp DEBIAN/copyright temp/DEBIAN
cp bin/Debug/*.* temp/usr/bin/
dpkg -b temp DEBIAN/mpmist.deb
cd DEBIAN
alien -r mpmist.deb
