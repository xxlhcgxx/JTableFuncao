﻿
$(document).ready(function () {

    if (document.getElementById("gridClientes"))
        $('#gridClientes').jtable({
            title: 'Clientes',
            paging: true, //Enable paging
            pageSize: 5, //Set page size (default: 10)
            sorting: true, //Enable sorting
            defaultSorting: 'Nome ASC', //Set default sorting
            actions: {
                listAction: urlClienteList,
            },
            fields: {
                Nome: {
                    title: 'Nome',
                    width: '30%'
                },
                Email: {
                    title: 'Email',
                    width: '25%'
                },
                CPF: {
                    title: 'CPF',
                    width: '25%'
                },
                Alterar: {
                    title: '',
                    width: '10%',
                    display: function (data) {
                        return '<button onclick="window.location.href=\'' + urlAlteracao + '/' + data.record.Id + '\'" class="btn btn-primary btn-sm">Alterar</button>';
                    }
                },
                Excluir: {
                    title: '',
                    width: '10%',
                    display: function (data) {
                        return '<button onclick="excluirCliente(' + data.record.Id + ')" class="btn btn-danger btn-sm">Excluir</button>';
                    }
                }
            }
        });

    //Load student list from server
    if (document.getElementById("gridClientes"))
        $('#gridClientes').jtable('load');
})

function excluirCliente(id) {
    var resposta = confirm("Deseja remover esse Cliente e seu(s) Beneficiário(s) ?");

    if (resposta == true) {
        $.ajax({
            url: urlExclusao,
            type: 'POST',
            dataType: 'json',
            data: {
                id: id
            },
            success: function (data) {
                $('#gridClientes').jtable('load');
            },
            error: function () {
            }
        });
    }
}