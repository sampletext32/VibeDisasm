const { app, BrowserWindow, ipcMain } = require('electron');
const path = require('path');
const url = require('url');

let mainWindow;

function createWindow() {
  app.commandLine.appendSwitch('disable-features', 'AutofillServerCommunication,PasswordGeneration');

  mainWindow = new BrowserWindow({
    width: 1200,
    height: 800,
    webPreferences: {
      nodeIntegration: true,
      contextIsolation: false,
      devTools: true || process.env.NODE_ENV === 'development'
    },
    autoHideMenuBar: true,
    frame: true
  });

  // Load the app
  const startUrl = process.env.ELECTRON_START_URL || url.format({
    pathname: path.join(__dirname, '../dist/vibe-disasm-front/browser/index.html'),
    protocol: 'file:',
    slashes: true
  });

  mainWindow.loadURL(startUrl)
    .then(() => {alert('Application loaded successfully!');})
    .catch(() => {alert('Failed to load the application. Please check the console for errors.');});

  // Open the DevTools in development mode
  if (true || process.env.NODE_ENV === 'development') {

    // Open DevTools with specific settings
    mainWindow.webContents.openDevTools({
      mode: 'bottom',
      activate: false
    });
  }

  // Emitted when the window is closed
  mainWindow.on('closed', function() {
    mainWindow = null;
  });
}

// Create window when Electron is ready
app.on('ready', createWindow);

// Quit when all windows are closed
app.on('window-all-closed', function() {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});

app.on('activate', function() {
  if (mainWindow === null) {
    createWindow();
  }
});
