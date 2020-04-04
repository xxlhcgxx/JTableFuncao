/* Nome da Procedure = FI_SP_VerificaCliente */

If Object_ID('FI_SP_VerificaCliente') Is Null
    Exec sp_executesql N'Create Procedure FI_SP_VerificaCliente As Set Nocount On';
Go

Alter Procedure FI_SP_VerificaCliente
	@CPF VARCHAR(14)	
AS
BEGIN
	SELECT 1 FROM CLIENTES WHERE CPF = Replace(Replace(@CPF, '.', ''), '-', '')
END