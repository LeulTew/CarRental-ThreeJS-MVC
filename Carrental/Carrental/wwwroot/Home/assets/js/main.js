import * as THREE from 'three';
import { GLTFLoader } from 'three/addons/loaders/GLTFLoader.js';
import { OrbitControls } from 'three/addons/controls/OrbitControls.js';

const renderer = new THREE.WebGLRenderer({ antialias: true });
renderer.outputColorSpace = THREE.SRGBColorSpace;

renderer.setSize(window.innerWidth, window.innerHeight);
renderer.setClearColor(0x000000);
renderer.setPixelRatio(window.devicePixelRatio);

renderer.shadowMap.enabled = true;
renderer.shadowMap.type = THREE.PCFSoftShadowMap;



document.body.prepend(renderer.domElement);

const scene = new THREE.Scene();
const camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 1, 1000);
camera.position.set(4, 5, 11);

const controls = new OrbitControls(camera, renderer.domElement);
controls.enableDamping = true;
controls.enablePan = false;
controls.minDistance = 5;
controls.enableZoom = false; // Disable zoom
controls.maxDistance = 20;
controls.minPolarAngle = 1.2;
controls.maxPolarAngle = 1.2;
controls.autoRotate = true;
controls.enableRotate = true; 
controls.target = new THREE.Vector3(0, 1, 0);
controls.update();


const radius = 10;
const segments = 32; // You can adjust the number of segments for smoother or more detailed circle

const groundGeometry = new THREE.CircleGeometry(radius, segments);
groundGeometry.rotateX(-Math.PI /2);
const groundMaterial = new THREE.MeshStandardMaterial({
  color: 0x808080,
  side: THREE.DoubleSide
});
const groundMesh = new THREE.Mesh(groundGeometry, groundMaterial);
groundMesh.castShadow = false;
groundMesh.receiveShadow = true;
scene.add(groundMesh);



const spotLight = new THREE.SpotLight(0xffffff,  3, 100, 0.22, 1);
spotLight.position.set(0, 25, 0);
spotLight.castShadow = true;
spotLight.shadow.bias = -0.0001;
scene.add(spotLight);


const modelPaths = [
  'car1',
  'car2',
    'car3',
  'car4'
];

let currentModelIndex = 0;
let currentModel;

function loadCurrentModel() {
    const loader = new GLTFLoader().setPath(modelPaths[currentModelIndex] + '/');
    loader.load('scene.gltf', (gltf) => {
    const newModel = gltf.scene;

    newModel.traverse((child) => {
      if (child.isMesh) {
        child.castShadow = true;
      }
    });
    const scaleFactor = window.innerWidth < 600 ? 0.7 : 1.5;
    newModel.scale.set(scaleFactor, scaleFactor, scaleFactor);

    newModel.position.set(0, groundMesh.position.y, 0);

    if (currentModel) {
      scene.remove(currentModel);
    }

    scene.add(newModel);
    currentModel = newModel;

    document.getElementById('progress-container').style.display = 'none';
  }, (xhr) => {
    // Progress callback
  });
}
window.addEventListener('resize', () => {
  camera.aspect = window.innerWidth / window.innerHeight;
  camera.updateProjectionMatrix();
  renderer.setSize(window.innerWidth, window.innerHeight);
  loadCurrentModel();
});



function setRendererSize() {
  const screenWidth = window.innerWidth;
  const screenHeight = window.innerHeight;
  renderer.setSize(screenWidth, screenHeight);
  camera.aspect = screenWidth / screenHeight;
  camera.updateProjectionMatrix();
}





// Event listeners
document.querySelector('.btn-arrow-right').addEventListener('click', () => {
  currentModelIndex = (currentModelIndex + 1) % modelPaths.length;
  loadCurrentModel();
});

document.querySelector('.btn-arrow-left').addEventListener('click', () => {
  currentModelIndex = (currentModelIndex - 1 + modelPaths.length) % modelPaths.length;
  loadCurrentModel();
});

// Initial load
loadCurrentModel();

function animate() {
  requestAnimationFrame(animate);
  controls.update();
  renderer.render(scene, camera);
}

animate();

