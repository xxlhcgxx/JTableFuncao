
function CarregarBeneficiarios() {

    if (!document.getElementById("gridBeneficiarios")) return;

    $('#formCadastro #CPF').mask('000.000.000-00', { placeholder: "Ex.: 010.011.111-00" });
    $('#formBeneficiario #CPFBenef').mask('000.000.000-00', { placeholder: "Ex.: 010.011.111-00" });

    LimparCampos();

    $('#gridBeneficiarios').jtable({
        title: 'Beneficiarios',
        paging: true,
        pageSize: 5,
        sorting: true,
        defaultSorting: 'Nome ASC',
        actions: {
            listAction: function (postData, jtParams) {
                return $.Deferred(function ($dfd) {
                    $.ajax({
                        url: urlBeneficiarioList + "?jtStartIndex=" + jtParams.jtStartIndex + '&jtPageSize=' + jtParams.jtPageSize + '&jtSorting=' + jtParams.jtSorting,
                        type: 'POST',
                        dataType: 'json',
                        data: postData,
                        success: function (data) {
                            window.beneficiarios = data.Records;
                            $dfd.resolve(data);
                        },
                        error: function () {
                            $dfd.reject();
                        }
                    });
                });
            },
        },
        fields: {
            CPF: {
                title: 'CPF',
                width: '40%'
            },
            Nome: {
                title: 'Nome',
                width: '40%'
            },
            Id: {
                key: true,
                visibility: "hidden"
            },
            Alterar: {
                title: '',
                width: '10%',
                display: function (data) {
                    return '<button onclick="alterarBeneficiario(\'' + data.record.Id + '\')" type="button" class="btn btn-primary btn-sm">Alterar</button>';
                }
            },
            Excluir: {
                title: '',
                width: '10%',
                display: function (data) {
                    return '<button onclick="excluirBeneficiario(\'' + data.record.Chave + '\',' + data.record.Id + ')" type="button" class="btn btn-danger btn-sm">Excluir</button>';
                }
            }
        }
    });

    if (document.getElementById("gridBeneficiarios"))
        $('#gridBeneficiarios').jtable('load');
}

function alterarBeneficiario(id) {
    var row = $("#gridBeneficiarios").jtable('getRowByKey', id);

    if (!row) return;

    var beneficiario = row.data().record;

    $("#IdBenef").val(beneficiario.Id);
    $("#NomeBenef").val(beneficiario.Nome);
    $("#CPFBenef").val(beneficiario.CPF);
}

function excluirBeneficiario(chave, id) {
    $.ajax({
        url: urlExclusaoBenef,
        type: 'POST',
        dataType: 'json',
        data: {
            chave: chave,
            id: id
        },
        success: function (data) {
            $('#gridBeneficiarios').jtable('load');
        },
        error: function (data) {
            $('#gridBeneficiarios').jtable('load');
        }
    });
}

function incluirBeneficiario() {

    var id = $('#formBeneficiario #IdBenef').val();
    var cpf = $('#formBeneficiario #CPFBenef').val();
    var nome = $('#formBeneficiario #NomeBenef').val();

    if (cpf == '' || nome == '') {
        alert('Para incluir/alterar um beneficiário, deve-se preencher as informações de Nome e CPF!')
        return;
    }

    $.ajax({
        url: urlInclusaoBenef,
        type: 'POST',
        dataType: 'json',
        data: {
            id: id,
            cpf: cpf,
            nome: nome
        },
        success: function (data) {
            if (data.Message != "") { alert(data.Message) }
            LimparCampos();
            $('#gridBeneficiarios').jtable('load');
        },
        error: function () {
        }
    });
}

function LimparCampos() {
    $('#formBeneficiario #IdBenef').val(0);
    $('#formBeneficiario #CPFBenef').val("");
    $('#formBeneficiario #NomeBenef').val("");
}