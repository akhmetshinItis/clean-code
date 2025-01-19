    document.getElementById('login-form').addEventListener('submit', async function(event) {
    event.preventDefault();

    const formData = new FormData(this);
    const data = {
    login: formData.get('login'),
    password: formData.get('password')
};

    const response = await fetch('/api/User/login', {
    method: 'POST',
    headers: {
    'Content-Type': 'application/json'
},
        body: JSON.stringify(data)
});

    if (response.ok) {
        location.href = '/index';
    }
});
