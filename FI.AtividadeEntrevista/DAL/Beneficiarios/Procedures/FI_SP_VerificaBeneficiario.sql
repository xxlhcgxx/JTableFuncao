/* Nome da Procedure = FI_SP_VerificaBeneficiario */

If Object_ID('FI_SP_VerificaBeneficiario') Is Null
    Exec sp_executesql N'Create Procedure FI_SP_VerificaBeneficiario As Set Nocount On';
Go

Alter Procedure FI_SP_VerificaBeneficiario
	@CPF VARCHAR(14)	
AS
BEGIN
	SELECT 1 FROM BENEFICIARIOS WHERE CPF = Replace(Replace(@CPF, '.', ''), '-', '')
END