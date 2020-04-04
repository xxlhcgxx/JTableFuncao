SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create FUNCTION FI_FN_FormatarCPF   (@Cpf char(14))
RETURNS CHAR(14)
AS
BEGIN

       DECLARE @retorno VARCHAR(14)

       IF LEN(@Cpf) = 11 
		   --CPF
		   BEGIN
                 SET @retorno = substring(@Cpf ,1,3) + '.' + substring(@Cpf ,4,3) + '.' + substring(@Cpf ,7,3) + '-' + substring(@Cpf ,10,2) 
           END
       RETURN @retorno
END
