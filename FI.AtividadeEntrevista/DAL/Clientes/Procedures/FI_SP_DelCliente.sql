/* Nome da Procedure = FI_SP_DelCliente */

If Object_ID('FI_SP_DelCliente') Is Null
    Exec sp_executesql N'Create Procedure FI_SP_DelCliente As Set Nocount On';
Go

Alter Procedure FI_SP_DelCliente
	@ID BIGINT
AS
BEGIN
	DELETE CLIENTES WHERE ID = @ID
END