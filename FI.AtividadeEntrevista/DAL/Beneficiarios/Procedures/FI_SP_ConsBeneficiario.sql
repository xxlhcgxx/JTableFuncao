/* Nome da Procedure = FI_SP_ConsBeneficiario */

If Object_ID('FI_SP_ConsBeneficiario') Is Null
    Exec sp_executesql N'Create Procedure FI_SP_ConsBeneficiario As Set Nocount On';
Go

Alter Procedure FI_SP_ConsBeneficiario
	@ID BIGINT
AS
BEGIN
	IF(ISNULL(@ID,0) = 0)
		SELECT NOME, dbo.FI_FN_FormatarCPF(CPF) CPF, ID, IDCLIENTE FROM BENEFICIARIOS WITH(NOLOCK)
	ELSE
		SELECT NOME, dbo.FI_FN_FormatarCPF(CPF) CPF, ID, IDCLIENTE FROM BENEFICIARIOS WITH(NOLOCK) WHERE ID = @ID
END