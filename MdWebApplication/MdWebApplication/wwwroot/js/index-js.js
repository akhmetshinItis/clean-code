// Инициализация CodeMirror для Markdown
const markdownEditor = CodeMirror.fromTextArea(document.getElementById("markdown-editor"), {
    mode: "markdown",
    lineNumbers: true,
    theme: "material",
});

// Инициализация CodeMirror для HTML
const htmlEditor = CodeMirror.fromTextArea(document.getElementById("html-editor"), {
    mode: "xml",
    lineNumbers: true,
    readOnly: true,
    theme: "material",
});

// Функция для автоматической конвертации Markdown в HTML
markdownEditor.on("change", async () => {
    const markdownInput = markdownEditor.getValue(); // Получаем содержимое Markdown редактора
    try {
        const response = await fetch('/api/Md/convert', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ markdown: markdownInput }),
        });

        if (response.ok) {
            const data = await response.json();
            htmlEditor.setValue(data.html); // Устанавливаем результат в HTML редактор
        } else {
            const error = await response.json();
            console.error('Error:', error.error);
        }
    } catch (err) {
        console.error('Failed to convert Markdown:', err);
    }
});

// Функция для извлечения значения cookie по имени
function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
}

// Функция для скачивания документа
document.getElementById('download-btn').addEventListener('click', async () => {
    const userId = getCookie('userId');  // Retrieve userId from cookies
    const fileName = document.getElementById('file-name-input').value.trim();  // Get file name from input field

    if (!userId) {
        alert('User ID is missing in cookies!');
        return;
    }

    if (!fileName) {
        alert('Please enter a file name!');
        return;
    }

    try {
        const response = await fetch(`/api/Document/download?userId=${userId}&fileName=${fileName}`, {
            method: 'GET',
        });

        if (response.ok) {
            const markdownContent = await response.text();
            markdownEditor.setValue(markdownContent);
        } else {
            const error = await response.json();
            alert(`Error: ${error.error}`);
        }
    } catch (err) {
        console.error('Failed to download document:', err);
        alert('An unexpected error occurred.');
    }
});

document.getElementById('save-form').addEventListener('submit', async (e) => {
    e.preventDefault();

    const fileName = document.getElementById('save-file-name').value.trim();
    const markdownContent = markdownEditor.getValue(); 
    const userId = getCookie('userId');

    if (!fileName) {
        alert('Please enter a file name.');
        return;
    }

    if (!markdownContent) {
        alert('Markdown content is empty.');
        return;
    }

    if (!userId) {
        alert('User ID is missing in cookies!');
        return;
    }

    try {
        const formData = new FormData();
        formData.append('file', markdownContent); // Добавляем содержимое Markdown
        formData.append('fileName', fileName); // Добавляем имя файла

        const response = await fetch(`/api/Document/upload?userId=${userId}`, {
            method: 'POST',
            body: formData,
        });

        if (response.ok) {
            const result = await response.text();
            alert(result); // Уведомляем пользователя об успехе
        } else {
            const error = await response.text();
            console.error('Error:', error);
            alert(`Error: ${error}`);
        }
    } catch (err) {
        console.error('Failed to upload document:', err);
        alert('An unexpected error occurred.');
    }
});

