If Not Exists(
	Select 1 From syscolumns Where id = object_id('CLIENTES') and name = 'CPF'
)
	Alter Table CLIENTES Add CPF Varchar(14) Not Null Default('')