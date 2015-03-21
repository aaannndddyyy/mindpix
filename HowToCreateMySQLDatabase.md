To install MySQL

```
    sudo apt-get install mysql-server
```

Run MySQL

```
    mysql -u root -p [enter password]
```

entering the root MySQL password which was given during installation.  Or alternatively

```
    sudo mysql
```

Now enter

```
    mysql> create database mindpixel;
```

to create a database called mindpixel.  The utilities assume that the database will have this name.  Now create a default user.

```
    mysql> grant usage on *.* to testuser@localhost identified by 'password';
    mysql> grant all privileges on mindpixel.* to testuser@localhost;
```

And exit from MySQL

```
    mysql> exit
```

Now to check user was created
```
    mysql -u testuser -p'password' mindpixel
```


**Some general tips on using MySQL**

To show databases:

```
    mysql> show databases;
```

Select the mindpixel database

```
    mysql> use mindpixel;
```

To show tables

```
    mysql> show tables;
```

To show table structure

```
    mysql> show columns from mindpixels;
```

To empty tables prior to importing GAC-80K

```
    mysql> DELETE FROM mindpixels;
    mysql> DELETE FROM users;
```