using FI.AtividadeEntrevista.BLL;
using FI.AtividadeEntrevista.DML;
using FI.AtividadeEntrevista.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebAtividadeEntrevista.Models;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        const string cadastroEfetuado = "Cadastro efetuado com sucesso!";
        const string cadastroAlterado = "Cadastro alterado com sucesso!";
        const string clienteExcluido = "Cliente excluído com sucesso!";
        const string clienteExcluidoErro = "Erro ao excluir Cliente!";

        const string cpfInvalido = "CPF Inválido!";
        const string cpfExistente = "CPF já existe na base de dados!";
        const string cpfIgual = "CPF do Cliente e Beneficiário não podem ser iguais!";

        public ActionResult Index()
        {
            LimparSessoes();
            return View();
        }

        public void LimparSessoes()
        {
            Session["IdCliente"] = 0;
            Session["ListaBeneficiario"] = new List<Beneficiario>();
            Session["ListaBeneficiarioExcluir"] = new List<Beneficiario>();
            Session["ListaBeneficiarioAlterar"] = new List<Beneficiario>();
            Session["ListaBeneficiarioIncluir"] = new List<Beneficiario>();
        }

        public ActionResult Incluir()
        {
            LimparSessoes();
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente boCli = new BoCliente();
            BoBeneficiario boBen = new BoBeneficiario();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                if (boCli.VerificarExistencia(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json(cpfExistente);
                }

                if (!ValidarCPF.ValidaCPF(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json(cpfInvalido);
                }

                if (((List<Beneficiario>)Session["ListaBeneficiario"]).Count > 0)
                {
                    var cpfIgual = ((List<Beneficiario>)Session["ListaBeneficiario"]).FirstOrDefault(c => c.CPF == model.CPF);
                    if (cpfIgual != null)
                    {
                        Response.StatusCode = 400;
                        return Json(cpfIgual);
                    }

                    bool isExiste = false; var dadosExiste = "";
                    bool isInvalido = false; var dadosInvalido = "";
                    foreach (var item in ((List<Beneficiario>)Session["ListaBeneficiario"]))
                    {
                        if (boBen.VerificarExistencia(item.CPF))
                        {
                            isExiste = true;
                            if (dadosExiste == "")
                            {
                                dadosExiste = "<br />" + "CPF do Beneficiario Existente: " + item.CPF.ToString();
                            }
                            else
                            {
                                dadosExiste = dadosExiste + "<br />"
                                    + "CPF do Beneficiario Existente: " + item.CPF.ToString();
                            }
                        }

                        if (!ValidarCPF.ValidaCPF(item.CPF))
                        {
                            isInvalido = true;
                            if (dadosInvalido == "")
                            {
                                dadosInvalido = "<br />" + "CPF do Beneficiario Inválido: " + item.CPF.ToString();
                            }
                            else
                            {
                                dadosInvalido = dadosInvalido + "<br />"
                                    + "CPF do Beneficiario Inválido: " + item.CPF.ToString();
                            }
                        }
                    }
                    if (isExiste)
                    {
                        Response.StatusCode = 400;
                        return Json(cpfExistente + "<br />" + dadosExiste.ToString());
                    }
                    if (isInvalido)
                    {
                        Response.StatusCode = 400;
                        return Json(cpfInvalido + "<br />" + dadosInvalido.ToString());
                    }
                }

                model.Id = boCli.Incluir(new Cliente()
                {
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF.Replace(".", "").Replace("-", "")
                });

                foreach (var item in ((List<Beneficiario>)Session["ListaBeneficiario"]))
                {
                    item.IdCliente = model.Id;
                    boBen.Incluir(item);
                }

                Session["IdCliente"] = model.Id;
                return Json(cadastroEfetuado);
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente boCli = new BoCliente();
            BoBeneficiario boBen = new BoBeneficiario();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                var dadosAntigo = boCli.Consultar(model.Id);
                if (dadosAntigo != null)
                {
                    if (dadosAntigo.CPF != model.CPF)
                    {
                        if (boCli.VerificarExistencia(model.CPF))
                        {
                            Response.StatusCode = 400;
                            return Json(cpfExistente);
                        }
                    }
                }

                var cpfIgual = ((List<Beneficiario>)Session["ListaBeneficiario"]).FirstOrDefault(c => c.CPF == model.CPF);
                if (cpfIgual != null)
                {
                    Response.StatusCode = 400;
                    return Json(cpfIgual);
                }

                if (!ValidarCPF.ValidaCPF(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json(cpfInvalido);
                }

                //Excluir dados
                var listaExcluir = ((List<Beneficiario>)Session["ListaBeneficiarioExcluir"]);
                if (listaExcluir.Count > 0)
                {
                    foreach (var item in listaExcluir)
                    {
                        boBen.Excluir(item.Id);
                    }
                }

                if (((List<Beneficiario>)Session["ListaBeneficiario"]).Count > 0)
                {
                    //Lista Incluir
                    bool isExiste = false; var dadosExiste = "";
                    bool isInvalido = false; var dadosInvalido = "";
                    var listaIncluir = ((List<Beneficiario>)Session["ListaBeneficiarioIncluir"]);
                    var listaBeneficiario = ((List<Beneficiario>)Session["ListaBeneficiario"]);
                    foreach (var item in listaIncluir)
                    {
                        if (boBen.VerificarExistencia(item.CPF))
                        {
                            isExiste = true;
                            if (dadosExiste == "")
                            {
                                dadosExiste = "<br />" + "CPF do Beneficiario Existente: " + item.CPF.ToString();
                            }
                            else
                            {
                                dadosExiste = dadosExiste + "<br />"
                                    + "CPF do Beneficiario Existente: " + item.CPF.ToString();
                            }

                            //Excluir da Lista de Beneficiarios
                            var dadosBen = listaBeneficiario.FirstOrDefault(c => c.CPF == item.CPF);
                            if (dadosBen != null) { listaBeneficiario.Remove(item); }

                            var dados = listaIncluir.FirstOrDefault(c => c.CPF == item.CPF);
                            if (dados != null) { listaIncluir.Remove(item); if (listaIncluir.Count == 0) { break; } }
                        }

                        if (!ValidarCPF.ValidaCPF(item.CPF))
                        {
                            isInvalido = true;
                            if (dadosInvalido == "")
                            {
                                dadosInvalido = "<br />" + "CPF do Beneficiario Inválido: " + item.CPF.ToString();
                            }
                            else
                            {
                                dadosInvalido = dadosInvalido + "<br />"
                                    + "CPF do Beneficiario Inválido: " + item.CPF.ToString();
                            }

                            //Excluir da Lista de Beneficiarios
                            var dadosBen = listaBeneficiario.FirstOrDefault(c => c.CPF == item.CPF);
                            if (dadosBen != null) { listaBeneficiario.Remove(item); }

                            var dados = listaIncluir.FirstOrDefault(c => c.CPF == item.CPF);
                            if (dados != null) { listaIncluir.Remove(item); if (listaIncluir.Count == 0) { break; } }
                        }
                    }
                    if (isExiste)
                    {
                        Response.StatusCode = 400;
                        return Json(cpfExistente + "<br />" + dadosExiste.ToString());
                    }
                    if (isInvalido)
                    {
                        Response.StatusCode = 400;
                        return Json(cpfInvalido + "<br />" + dadosInvalido.ToString());
                    }
                }

                boCli.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF.Replace(".", "").Replace("-", "")
                });

                foreach (var item in ((List<Beneficiario>)Session["ListaBeneficiarioIncluir"]))
                {
                    item.IdCliente = model.Id;
                    boBen.Incluir(item);
                }

                foreach (var item in ((List<Beneficiario>)Session["ListaBeneficiarioAlterar"]))
                {
                    var antigo = boBen.Consultar(item.Id);
                    if (antigo != null)
                    {
                        antigo.Nome = item.Nome;
                        antigo.CPF = item.CPF;
                        boBen.Alterar(antigo);
                    }
                }

                return Json(cadastroAlterado);
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            LimparSessoes();
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            ClienteModel model = null;

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    CPF = cliente.CPF
                };

                Session["IdCliente"] = cliente.Id;
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult Excluir(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);

            if (cliente != null)
            {
                BoBeneficiario boBen = new BoBeneficiario();
                var listaBeneficiario = boBen.Pesquisa(0, 5, "", true, id, out int qtd);
                foreach (var item in listaBeneficiario)
                {
                    boBen.Excluir(item.Id);
                }

                bo.Excluir(cliente.Id);
                Session["IdCliente"] = 0;

                Response.StatusCode = 200;
                return Json(clienteExcluido);
            }

            Response.StatusCode = 400;
            return Json(clienteExcluidoErro);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult BeneficiarioList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                if (((List<Beneficiario>)Session["ListaBeneficiario"]).Count > 0)
                {
                    return Json(new
                    {
                        Result = "OK",
                        Records = ((List<Beneficiario>)Session["ListaBeneficiario"]),
                        TotalRecordCount = ((List<Beneficiario>)Session["ListaBeneficiario"]).Count
                    });
                }

                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Beneficiario> beneficiarios = new List<Beneficiario>();
                if (Session["IdCliente"].ToString() != "0")
                {
                    if ((long)Session["IdCliente"] > 0)
                    {
                        beneficiarios = new BoBeneficiario().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), (long)Session["IdCliente"], out qtd);
                        Session["ListaBeneficiario"] = beneficiarios;
                    }

                    if (((List<Beneficiario>)Session["ListaBeneficiarioExcluir"]).Count > 0)
                    {
                        var listaExcluir = ((List<Beneficiario>)Session["ListaBeneficiarioExcluir"]);
                        foreach (var item in beneficiarios.ToList())
                        {
                            var cpfIgual = beneficiarios.FirstOrDefault(c => c.CPF == item.CPF);
                            if (cpfIgual != null)
                            {
                                beneficiarios.Remove(cpfIgual);
                            }
                        }
                    }
                }
                return Json(new { Result = "OK", Records = beneficiarios, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ExcluirBeneficiario(Guid chave, int id)
        {
            try
            {
                if (((List<Beneficiario>)Session["ListaBeneficiario"]).Count > 0)
                {
                    if (id > 0)
                    {
                        ((List<Beneficiario>)Session["ListaBeneficiarioExcluir"]).Add(new Beneficiario
                        {
                            Id = id
                        });
                    }
                    var listaBenef = ((List<Beneficiario>)Session["ListaBeneficiario"]);
                    var item = listaBenef.FirstOrDefault(c => c.Chave == chave);
                    if (item != null) { listaBenef.Remove(item); }

                    Session["ListaBeneficiario"] = listaBenef;

                    return Json(new
                    {
                        Result = "OK",
                        Records = ((List<Beneficiario>)Session["ListaBeneficiario"]),
                        TotalRecordCount = ((List<Beneficiario>)Session["ListaBeneficiario"]).Count
                    });
                }

                return Json(new
                {
                    Result = "OK",
                    Records = ((List<Beneficiario>)Session["ListaBeneficiario"]),
                    TotalRecordCount = ((List<Beneficiario>)Session["ListaBeneficiario"]).Count
                });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult IncluirAlterarBeneficiario(int id, string cpf, string nome)
        {
            try
            {
                BoBeneficiario boBen = new BoBeneficiario();

                if (cpf.ToString() == "" || nome.ToString() == "")
                {
                    return Json(new
                    {
                        Result = "OK",
                        Records = ((List<Beneficiario>)Session["ListaBeneficiario"]),
                        TotalRecordCount = ((List<Beneficiario>)Session["ListaBeneficiario"]).Count
                    });
                }

                if (id == 0)
                {
                    if (boBen.VerificarExistencia(cpf))
                    {
                        return Json(new { Error = true, Result = "ERROR", Message = "CPF Existente na base de Beneficiário!" });
                    }
                    var listaExpe = ((List<Beneficiario>)Session["ListaBeneficiario"]).FirstOrDefault(c => c.CPF == cpf && c.Id == 0);
                    if (listaExpe != null)
                    {
                        var listaBenef = ((List<Beneficiario>)Session["ListaBeneficiario"]);
                        var listaDup = ((List<Beneficiario>)Session["ListaBeneficiario"]).FirstOrDefault(c => c.CPF == cpf);
                        if (listaDup != null) { listaBenef.Remove(listaDup); }
                    }

                    ((List<Beneficiario>)Session["ListaBeneficiarioIncluir"]).Add(new Beneficiario
                    {
                        CPF = cpf,
                        Nome = nome,
                    });

                    ((List<Beneficiario>)Session["ListaBeneficiario"]).Add(new Beneficiario
                    {
                        CPF = cpf,
                        Nome = nome,
                    });
                }
                else
                {

                    var dadosAntigo = boBen.Consultar(id);
                    if (dadosAntigo != null)
                    {
                        if (dadosAntigo.CPF != cpf)
                        {
                            if (boBen.VerificarExistencia(cpf))
                            {
                                return Json(new { Erro = true, Result = "ERROR", Message = "CPF Existente na base de Beneficiário!" });
                            }

                            var listaExpe = ((List<Beneficiario>)Session["ListaBeneficiario"]).FirstOrDefault(c => c.CPF == cpf);
                            if (listaExpe != null)
                            {
                                return Json(new { Result = "ERROR", Message = "CPF Existente na base de Beneficiário!" });
                            }
                        }
                    }

                    ((List<Beneficiario>)Session["ListaBeneficiarioAlterar"]).Add(new Beneficiario
                    {
                        Id = id,
                        CPF = cpf,
                        Nome = nome,
                        IdCliente = (long)Session["IdCliente"]
                    });

                    var listaBenef = ((List<Beneficiario>)Session["ListaBeneficiario"]);
                    foreach (var item in listaBenef.Where(c => c.Id == id))
                    {
                        item.Nome = nome;
                        item.CPF = cpf;
                    }
                    Session["ListaBeneficiario"] = listaBenef;
                }

                return Json(new
                {
                    Result = "OK",
                    Records = ((List<Beneficiario>)Session["ListaBeneficiario"]),
                    TotalRecordCount = ((List<Beneficiario>)Session["ListaBeneficiario"]).Count,
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}