/* Nome da Procedure = FI_SP_PesqBeneficiario */

If Object_ID('FI_SP_PesqBeneficiario') Is Null
    Exec sp_executesql N'Create Procedure FI_SP_PesqBeneficiario As Set Nocount On';
Go

Alter Procedure FI_SP_PesqBeneficiario
	@iniciarEm int,
	@quantidade int,
	@campoOrdenacao varchar(200),
	@crescente bit,
	@idCliente BigInt
AS
BEGIN
	DECLARE @SCRIPT NVARCHAR(MAX)
	DECLARE @CAMPOS NVARCHAR(MAX)
	DECLARE @ORDER VARCHAR(50)
	
	IF(@campoOrdenacao = 'NOME')
		SET @ORDER =  ' NOME '
	ELSE
		SET @ORDER = ' CPF '

	IF(@crescente = 0)
		SET @ORDER = @ORDER + ' DESC'
	ELSE
		SET @ORDER = @ORDER + ' ASC'

	SET @CAMPOS = '@iniciarEm int,@quantidade int'
	SET @SCRIPT = 
	'SELECT ID, NOME, dbo.FI_FN_FormatarCPF(CPF) CPF, IDCLIENTE FROM
		(SELECT ROW_NUMBER() OVER (ORDER BY ' + @ORDER + ') AS Row, ID, NOME, CPF, IDCLIENTE FROM BENEFICIARIOS WITH(NOLOCK))
		AS BeneficiariosWithRowNumbers
	WHERE Row > @iniciarEm AND Row <= (@iniciarEm+@quantidade) '
	
	if (@idCliente > 0)
		SET @SCRIPT = @SCRIPT + 'AND IDCLIENTE = ' + Convert(Varchar(10),@idCliente)

	SET @SCRIPT = @SCRIPT + ' ORDER BY' + @ORDER
	
	print @SCRIPT
	EXECUTE SP_EXECUTESQL @SCRIPT, @CAMPOS, @iniciarEm, @quantidade

	SELECT COUNT(1) FROM BENEFICIARIOS WITH(NOLOCK)
END