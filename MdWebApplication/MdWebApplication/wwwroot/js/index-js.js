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

// Функция для скачивания документа
async function downloadDocument(fileName) {
    if (!fileName) {
        alert('File name is missing!');
        return;
    }

    try {
        const response = await fetch(`/api/Document/download?fileName=${fileName}`, {
            method: 'GET',
        });

        if (response.ok) {
            const markdownContent = await response.text();
            markdownEditor.setValue(markdownContent); // Отображение содержимого в редакторе
        } else {
            const error = await response.json();
            alert(`Error: ${error.error}`);
        }
    } catch (err) {
        console.error('Failed to download document:', err);
        alert('An unexpected error occurred.');
    }
}

document.getElementById('save-form').addEventListener('submit', async (e) => {
    e.preventDefault();

    const fileName = document.getElementById('save-file-name').value.trim();
    const markdownContent = markdownEditor.getValue();
    
    if (!fileName) {
        alert('Please enter a file name.');
        return;
    }

    if (!markdownContent) {
        alert('Markdown content is empty.');
        return;
    }

    try {
        const formData = new FormData();
        formData.append('file', markdownContent); // Добавляем содержимое Markdown
        formData.append('fileName', fileName); // Добавляем имя файла

        const response = await fetch(`/api/Document/upload`, {
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

document.getElementById('load-documents-btn').addEventListener('click', async () => {
    try {
        const response = await fetch(`/api/Document/all`, { method: 'GET' });

        if (response.ok) {
            const documents = await response.json();
            const documentsList = document.getElementById('documents-list');

            documentsList.innerHTML = '';

            documents.forEach(doc => {
                const listItem = document.createElement('li');
                listItem.innerHTML = `
                    <strong>${doc.documentName}</strong> -
                    <input type="hidden" id="file-name-input" value="${doc.documentName}" style="margin-right: 10px;">
                    <button class="download-btn" data-filename="${doc.documentName}" style="margin-top: 20px;">Download Document</button>
                `;
                documentsList.appendChild(listItem);

                const downloadButtons = document.querySelectorAll('.download-btn');
                downloadButtons.forEach(button => {
                    button.addEventListener('click', async (event) => {
                        const fileName = event.target.getAttribute('data-filename');
                        await downloadDocument(fileName);
                    });
                });
                
            });
        } else {
            const error = await response.json();
            console.error('Error:', error.error);
            alert('Failed to load documents: ' + error.error);
        }
    } catch (err) {
        console.error('Failed to fetch documents:', err);
        alert('Error fetching documents.');
    }
});

document.getElementById('logout-btn').addEventListener('click', async () => {
    try {
        const response = await fetch('/api/user/logout', {
            method: 'POST',
        });

        if (response.ok) {
            alert('Logged out successfully');
            // Опционально можно перенаправить пользователя на страницу входа
            window.location.href = '/login';
        } else {
            alert('Failed to log out');
        }
    } catch (err) {
        console.error('Error during logout:', err);
        alert('An unexpected error occurred during logout.');
    }
});
