const { spawn } = require('child_process');
const waitOn = require('wait-on');
const electron = require('electron');

// Start Angular development server
console.log('Starting Angular development server...');
const ngServe = spawn('npm', ['run', 'start'], {
  shell: true,
  stdio: 'inherit'
});

// Wait for Angular server to be ready
waitOn({
  resources: ['http://localhost:4200'],
  timeout: 30000 // 30 seconds timeout
}).then(() => {
  console.log('Angular server is ready. Starting Electron...');
  
  // Start Electron
  const electronProcess = spawn(electron, ['.'], {
    shell: true,
    stdio: 'inherit',
    env: {
      ...process.env,
      ELECTRON_START_URL: 'http://localhost:4200',
      NODE_ENV: 'development'
    }
  });
  
  // Handle Electron exit
  electronProcess.on('close', () => {
    ngServe.kill();
    process.exit();
  });
}).catch((err) => {
  console.error('Error starting:', err);
  ngServe.kill();
  process.exit(1);
});

// Handle process termination
process.on('SIGINT', () => {
  ngServe.kill();
  process.exit(0);
});
