To create a new database:

mysql -u root -p [enter password]
mysql> create database mindpixel;
mysql> grant usage on *.* to testuser@localhost identified by 'password';
mysql> grant all privileges on mindpixel.* to testuser@localhost;
mysql> exit

To check user was created:

    mysql -u testuser -p'password' mindpixel

To show databases:

    mysql> show databases;

Select the mindpixel database:

    mysql> use mindpixel;

To show tables:

    mysql> show tables;

To show table structure:

    mysql> show columns from mindpixels;