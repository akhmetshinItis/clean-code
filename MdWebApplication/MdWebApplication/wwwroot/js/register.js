document.getElementById('register-form').addEventListener('submit', async function(event) {
    event.preventDefault();

    const formData = new FormData(this);
    const data = {
        username: formData.get('username'),
        login: formData.get('login'),
        password: formData.get('password')
    };

    try {
        const response = await fetch('/api/User/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            location.href = '/index';
        } else {
            alert('Registration failed. Please try again.');
        }
    } catch (err) {
        console.error('Registration error:', err);
        alert('An unexpected error occurred. Please try again.');
    }
});