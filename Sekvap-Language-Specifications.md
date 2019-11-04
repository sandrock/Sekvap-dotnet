
Sekvap language specifications
===============================

Version 1.

Key
------

A key IS a string.  
A key MAY be an empty string.

Escape ';' as '\;'.  
Escape '=' as '\='.  

Value
------

A value IS a string.
A value MAY be an empty string.

A value IS attached to a key using the '=' char. 

Escape ';' as '\;'.  
Escape '=' as '\='.  

Kevap
------

A kevap IS EITHER:

- a heading value without a key
- a key 
- a key and a value

Kevaps MUST be separater using the ';' char.


Sekvap
-------

A sekvap string IS a collection of separated kevaps.  

The first kevap MAY not have a key. Thus the unspecified key is the 'Value' string.


