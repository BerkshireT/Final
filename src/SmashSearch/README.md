paket install
paket restore && paket generate-load-scripts -t fsx
yarn
yarn start