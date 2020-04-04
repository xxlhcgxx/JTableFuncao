/* Nome da Procedure = FI_SP_AltBeneficiario */

If Object_ID('FI_SP_AltBeneficiario') Is Null
    Exec sp_executesql N'Create Procedure FI_SP_AltBeneficiario As Set Nocount On';
Go

Alter Procedure FI_SP_AltBeneficiario
    @NOME          VARCHAR (50) ,
	@CPF           VARCHAR (14),
	@Id            BIGINT
AS
BEGIN
	UPDATE BENEFICIARIOS 
	SET 
		NOME = @NOME, 
		CPF = Replace(Replace(@CPF, '.', ''), '-', '')
	WHERE Id = @Id
END
