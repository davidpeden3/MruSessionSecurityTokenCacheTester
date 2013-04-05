MruSessionSecurityTokenCacheTester
==================================

Demonstrates a disconnect in the add and get APIs of MruSessionSecurityTokenCache

It is possible to add a SessionSecurityToken into an MruSessionSecurityTokenCache and not be able to retrieve it using the GetAll method.

Either GetAll should be fixed to tolerate String.Empty for endpoint IDs or SessionSecurityToken's constructors should not allow String.Empty as they don't allow null.
