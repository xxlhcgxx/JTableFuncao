/* Nome da Procedure = FI_SP_IncBeneficiario */

If Object_ID('FI_SP_IncBeneficiario') Is Null
    Exec sp_executesql N'Create Procedure FI_SP_IncBeneficiario As Set Nocount On';
Go

Alter Procedure FI_SP_IncBeneficiario
    @NOME          VARCHAR (50) ,
	@CPF           VARCHAR (14) ,
	@IDCLIENTE     BIGINT
AS
BEGIN
	INSERT INTO BENEFICIARIOS (NOME, CPF, IDCLIENTE) 
	VALUES (@NOME, Replace(Replace(@CPF, '-', ''), '.', ''), @IDCLIENTE)

	SELECT SCOPE_IDENTITY()
END