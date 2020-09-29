# Setup a training environment

using mlagents release 6, package manager mlagents:1.0.5, Unity 2019.4.10f1

1. conda env list
2. conda create -n "name" python=3.8.1
3. conda activate "name"
4. pip install mlagents==0.19.0
5. mlagents-learn "config" --run-id="aName_0x"
