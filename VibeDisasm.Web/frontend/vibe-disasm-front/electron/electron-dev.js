const { spawn } = require('child_process');
const waitOn = require('wait-on');
const electron = require('electron');
const net = require('net');

// Check if port 4200 is already in use
function isPortInUse(port) {
  return new Promise((resolve) => {
    const client = new net.Socket();
    
    client.once('connect', () => {
      // Connection successful, port is in use
      client.destroy();
      resolve(true);
    });
    
    client.once('error', () => {
      // Connection failed, port is not in use
      client.destroy();
      resolve(false);
    });
    
    // Attempt to connect to the port
    client.connect(port, 'localhost');
  });
}

// Variable to track if we started Angular ourselves
let angularStartedByUs = false;
let ngServe;

// Check if Angular is already running
isPortInUse(4200).then(portInUse => {
  if (portInUse) {
    console.log('Angular server is already running on port 4200. Using existing server...');
    // Double check that the server is actually responding to HTTP requests
    waitOn({
      resources: ['http://localhost:4200'],
      timeout: 5000 // 5 seconds timeout
    }).then(() => {
      console.log('Confirmed Angular server is responding...');
      startElectron();
    }).catch(() => {
      console.log('Port is in use but Angular server is not responding. Starting new server...');
      startAngularServer();
    });
  } else {
    startAngularServer();
  }
});

// Function to start Angular server
function startAngularServer() {
  // Start Angular development server
  console.log('Starting Angular development server...');
  ngServe = spawn('npm', ['run', 'start'], {
    shell: true,
    stdio: 'inherit'
  });
  angularStartedByUs = true;
  
  // Wait for Angular server to be ready
  waitOn({
    resources: ['http://localhost:4200'],
    timeout: 30000 // 30 seconds timeout
  }).then(startElectron).catch((err) => {
    console.error('Error starting:', err);
    if (ngServe) ngServe.kill();
    process.exit(1);
  });
}

// Function to start Electron
function startElectron() {
  console.log('Starting Electron with Angular server at http://localhost:4200...');
  
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
    if (angularStartedByUs && ngServe) {
      ngServe.kill();
    }
    process.exit();
  });
}

// Handle process termination
process.on('SIGINT', () => {
  if (angularStartedByUs && ngServe) {
    ngServe.kill();
  }
  process.exit(0);
});
