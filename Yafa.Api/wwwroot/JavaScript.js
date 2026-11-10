var origin = window.location.origin;
var path = window.location.pathname.toLowerCase();

var pathPrefix = '';

if (path.startsWith('/yafa')) {
    pathPrefix = '/yafa';
} else if (path.startsWith('/api')) {
    pathPrefix = '/api';
}

var _BaseURL = origin + pathPrefix;
console.log(_BaseURL);

// ── helpers ───────────────────────────────────────────────────────────
function showError(msg) {
    $('#alertMsg').text(msg);
    $('#alertArea').fadeIn(200);
}

function hideError() {
    $('#alertArea').fadeOut(150);
}

function setLoading(loading) {
    const $btn = $('#loginBtn');
    const $text = $('#btnText');
    if (loading) {
        $btn.prop('disabled', true);
        $text.html('<span class="spinner-sm"></span>Signing in…');
    } else {
        $btn.prop('disabled', false);
        $text.text('Sign In');
    }
}

// ── password toggle ────────────────────────────────────────────────────
$('#pwdToggle').on('click', function () {
    const $input = $('#password');
    const $icon = $('#pwdIcon');
    if ($input.attr('type') === 'password') {
        $input.attr('type', 'text');
        $icon.removeClass('bi-eye').addClass('bi-eye-slash');
    } else {
        $input.attr('type', 'password');
        $icon.removeClass('bi-eye-slash').addClass('bi-eye');
    }
});

// ── form submit ────────────────────────────────────────────────────────
$('#loginForm').on('submit', function (e) {
    e.preventDefault();
    hideError();

    const email = $.trim($('#email').val());
    const password = $.trim($('#password').val());

    // Basic client-side validation
    if (!email) { showError('Email address is required.'); return; }
    if (!password) { showError('Password is required.'); return; }

    const rememberMe = $('#rememberMe').is(':checked');

    const payload = {
        email: email,
        password: password,
        rememberMe: rememberMe,
        deviceToken: 'WEB_ADMIN_PORTAL',
        deviceTokenPlatform: 'WEB'
    };

    setLoading(true);

    $.ajax({
        url: _BaseURL + '/api/AdminAuth/admin-login-for-access-api',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload),
        success: function (res) {
            if (res.isSuccess) { 
                window.location.href = _BaseURL + '/scalar-portal';
            } else {
                setLoading(false);
                showError((res && res.errorMessages && res.errorMessages.length)
                    ? res.errorMessages[0]
                    : 'Invalid email or password.');
            }
        },
        error: function (xhr) {
            setLoading(false);
            let msg = 'An error occurred. Please try again.';
            try {
                const err = JSON.parse(xhr.responseText);
                if (err && err.errorMessages && err.errorMessages.length)
                    msg = err.errorMessages[0];
            } catch (_) { }
            showError(msg);
        }
    });
});

// ── auto-focus email on load ──────────────────────────────────────────
$(document).ready(function () {
    $.ajax({
        url: _BaseURL + '/api/AdminAuth/check',
        method: 'GET',
        xhrFields: {
            withCredentials: true
        },
        success: function() {
            // Already logged in → redirect
            window.location.href = _BaseURL + '/scalar-portal';
        },
        error: function() {
            // Not logged in → stay on login page
            $('#email').focus();
        }
    });
});