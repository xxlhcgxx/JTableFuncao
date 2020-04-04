/* Nome da Procedure = FI_SP_DelBeneficiario */

If Object_ID('FI_SP_DelBeneficiario') Is Null
    Exec sp_executesql N'Create Procedure FI_SP_DelBeneficiario As Set Nocount On';
Go

Alter Procedure FI_SP_DelBeneficiario
	@ID BIGINT
AS
BEGIN
	DELETE BENEFICIARIOS WHERE ID = @ID
END