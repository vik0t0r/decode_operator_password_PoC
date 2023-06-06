# Issue
Improper hashing of the Operators password in the salto database allows an attacker with access to it to recover the passwords from its hashes, as if they were stored in cleartext.
# How to use:
1. Modify the database connection string to match yours
2. Compile and run the project (I have used jetbrains raider, but you should be able to compile with visual studio too)
# Expected output:
```
SysCode (stored in DB): XXXXXXXXXXXXXXXX
SysCode (cleartex, 32bits): AAAAAAAA
Id: 1, Name: admin, log_username: admin, log_password: XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX, cleartext_password: Admin1234
Id: 2, Name: _PMS, log_username: xxxxxxxxxxxx, log_password: XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX, cleartext_password: ERROR RECOVERING
```
Only users with password (ej: _PMS not) are recoverable
# Explanation
The algorithm using to calculate password hashes is as follows, an implementation can be found [here](fixme):
1. Get syscode from db (Where it is ciphered with a static key found on the code)
2. Calculate a salt Xoring the syscode with the id operator (I guess this is done to get a different salt for each operator)
3. Convert the password string to a byte array, padded to 16 bytes
4. Cipher the password using AES with CredentialKey and CredentialIV, both found in the source code
5. Calculate the sha512 hash of the salt 
6. Xor the ciphered password with the hash of the salt
7. Store xor result on the database

The only step which is not reversible is the calculation of the sha512 hash, but we already know that its cleartext is the syscode xored with the operator id.


To properly implement this, first the salt should be appended to the password and the result should be hashed with sha512.


The ciphering process with AES, its not adding more security as the password is always the same (stored in source code), I would suggest removing it altogether.