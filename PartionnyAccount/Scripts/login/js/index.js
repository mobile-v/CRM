// Toggle Function
$('.toggle').click(function(){
  
  // Switches the Icon
  $(this).children('i').toggleClass('fa-pencil');
  // Switches the forms  
  $('.form').animate({
    height: "toggle",
    'padding-top': 'toggle',
    'padding-bottom': 'toggle',
    opacity: "toggle"
  }, "slow");
  

    //document.location.href = 'account/registration/';
});







//������� ��� "PswdTemp"
function fn_password_onKeyPress() {
    
    //�������� ��������� ������
    var sSymbol = document.getElementById('PswdTemp').value[document.getElementById('PswdTemp').value.length - 1];

    //��������, ���� �������� ������ "*" - �����
    if (sSymbol == "*") { return; }

    //������
    if (document.getElementById('PswdTemp').value.length > 1) document.getElementById('Pswd').value += sSymbol;
    else document.getElementById('Pswd').value = sSymbol;

    var sPassZvezda = "";
    for (var i = 0; i < document.getElementById('Pswd').value.length; i++) {
        sPassZvezda += "**"
    }

    document.getElementById('PswdTemp').value = sPassZvezda;
}
function fn_password_onKeyPress2() {
    
    //�������� ��������� ������
    var sSymbol = document.getElementById('PswdTemp2').value[document.getElementById('PswdTemp2').value.length - 1];

    //��������, ���� �������� ������ "*" - �����
    if (sSymbol == "*") { return; }

    //������
    if (document.getElementById('PswdTemp2').value.length > 1) document.getElementById('Pswd2').value += sSymbol;
    else document.getElementById('Pswd2').value = sSymbol;

    var sPassZvezda = "";
    for (var i = 0; i < document.getElementById('Pswd2').value.length; i++) {
        sPassZvezda += "**"
    }

    document.getElementById('PswdTemp2').value = sPassZvezda;
}

//������� ��� ������� ������ "����"
/*function fn_button_onclick() {
    
    alert(document.getElementById('Pswd').value);
    document.getElementById('Pswd').value = document.getElementById('PswdTemp').value;
    document.getElementById('PswdTemp').value = "*";
}*/

//������� ��� ����� ����������
function fn_Change_DirThemeID(DirThemeID) {
    if (document.getElementById('DirThemeID').value >= 3) {
        document.getElementById('DirInterfaceID').value = 3;
        alert("��� ������ ���� �������� ������ ��������� ������");
    }
}
