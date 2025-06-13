# Batch-Export-Meshes-from-Generated-Scenes-by-ProcTHOR

> [!TIP]
> These scripts are designed to create large-scale scene meshes for robot learning. Our work for generalizable indoor active mapping can be found in https://github.com/zjwzcx/GLEAM.

## 📋 Overview
We provide automated scripts for batch exporting `.fbx` meshes from ProcTHOR-format generated scene descriptions (e.g., ProcTHOR-10K JSON files). This pipeline facilitates large-scale dataset creation for robotics and computer vision research.



## 🛠️ Software

We test our code under the following environment:
- Unity Hub 3.11.0
- Unity Editor 2021.3.45f1
	- FBX exporter 4.1.0 (Unity package. You can install it in Unity's package manager.)


## 🕹️ Get Started

1. Clone the AI2-THOR repository:

```bash
git clone https://github.com/allenai/ai2thor.git
```

2. Merge provided scripts into AI2-THOR file structure:


```
ai2thor
└── unity
    └── Assets
        ├── Editor
        │   └── BatchImportExportProcthor.cs  # Add this file
        └── Scripts
            └── DebugInputField.cs            # Replace existing file
```

3. Configuration

You can configure paths in BatchImportExportProcthor.cs. The default settings are as follows:

```bash
string roomsPath = "ai2thor/scenes_json";		# json path
string exportFBXPath = "Assets/ExportedFBX";	# export path
```

Place your ProcTHOR scene JSON files in the specified `roomsPath`.

4. Execution

- In Unity Hub: Add existing project from ai2thor/unity directory.
- Navigate to: Tools > Batch_Convert_JSON_To_FBX
- Export progress will display in Unity Console


## 📊 GLEAM-Bench Dataset

We've created hundreds of ProcTHOR scene meshes by our scripts, and also curate diverse indoor scenes from datasets including HSSD, Gibson and Matterport3D. If you're interested in the dataset, please check it out:

<p align="center">
  <img src="assets/overview_gleambench.png" align="center" width="100%">
</p>
<p align="center">
  <img src="assets/statistic.png" align="center" width="100%">
</p>

**GLEAM-Bench** includes 1,152 diverse 3D scenes from synthetic and real-scan datasets for benchmarking generalizable active mapping policies. These curated scene meshes are characterized by near-watertight geometry, diverse floorplan (≥10 types), and complex interconnectivity. We unify these multi-source datasets through filtering, geometric repair, and task-oriented preprocessing. Please refer to the **[guide](https://github.com/zjwzcx/GLEAM/blob/master/data_gleam/README.md)** for more details and scrips.

We provide all the preprocessed data used in our work, including mesh files (in `obj` folder), ground-truth surface points (in `gt` folder) and asset indexing files (in `urdf` folder). We recommend users fill out the form to access the **download link [[HERE](https://docs.google.com/forms/d/e/1FAIpQLSdq9aX1dwoyBb31nm8L_Mx5FeaVsr5AY538UiwKqg8LPKX9vg/viewform?usp=sharing)]**. 


## 🙏 Acknowledgments

We thank ProcTHOR team and Allen Institute to provide the awesome projects including ProcTHOR and ai2thor. 

Our scripts benefit a lot from the discussion in issues (https://github.com/allenai/ai2thor/issues/1128).

## 📜 Citation

If you find our scripts and provided dataset helpful, please cite them:

```bibtex
@misc{chen2025gleam,
  title={GLEAM: Learning Generalizable Exploration Policy for Active Mapping in Complex 3D Indoor Scenes},
  author={Xiao Chen and Tai Wang and Quanyi Li and Tao Huang and Jiangmiao Pang and Tianfan Xue},
  year={2025},
  eprint={2505.20294},
  archivePrefix={arXiv},
  primaryClass={cs.CV},
  url={https://arxiv.org/abs/2505.20294}, 
}
```

If you use our codebase, dataset and benchmark, please kindly cite the original datasets involved in our work. BibTex entries are provided below.

<details><summary>Dataset BibTex</summary>

```bibtex
@inproceedings{chen2024gennbv,
  title={GenNBV: Generalizable Next-Best-View Policy for Active 3D Reconstruction},
  author={Chen, Xiao and Li, Quanyi and Wang, Tai and Xue, Tianfan and Pang, Jiangmiao},
  year={2024}
  booktitle={IEEE Conference on Computer Vision and Pattern Recognition (CVPR)},
}
```
```bibtex
@article{ai2thor,
  author={Eric Kolve and Roozbeh Mottaghi and Winson Han and
          Eli VanderBilt and Luca Weihs and Alvaro Herrasti and
          Daniel Gordon and Yuke Zhu and Abhinav Gupta and
          Ali Farhadi},
  title={{AI2-THOR: An Interactive 3D Environment for Visual AI}},
  journal={arXiv},
  year={2017}
}
```
```bibtex
@inproceedings{rudin2022learning,
  title={Learning to walk in minutes using massively parallel deep reinforcement learning},
  author={Rudin, Nikita and Hoeller, David and Reist, Philipp and Hutter, Marco},
  booktitle={Conference on Robot Learning},
  pages={91--100},
  year={2022},
  organization={PMLR}
}
```
```bibtex
@inproceedings{procthor,
  author={Matt Deitke and Eli VanderBilt and Alvaro Herrasti and
          Luca Weihs and Jordi Salvador and Kiana Ehsani and
          Winson Han and Eric Kolve and Ali Farhadi and
          Aniruddha Kembhavi and Roozbeh Mottaghi},
  title={{ProcTHOR: Large-Scale Embodied AI Using Procedural Generation}},
  booktitle={NeurIPS},
  year={2022},
  note={Outstanding Paper Award}
}
```
```bibtex
@inproceedings{xiazamirhe2018gibsonenv,
  title={Gibson Env: real-world perception for embodied agents},
  author={Xia, Fei and R. Zamir, Amir and He, Zhi-Yang and Sax, Alexander and Malik, Jitendra and Savarese, Silvio},
  booktitle={Computer Vision and Pattern Recognition (CVPR), 2018 IEEE Conference on},
  year={2018},
  organization={IEEE}
}
```
```bibtex
@article{khanna2023hssd,
    author={Khanna*, Mukul and Mao*, Yongsen and Jiang, Hanxiao and Haresh, Sanjay and Shacklett, Brennan and Batra, Dhruv and Clegg, Alexander and Undersander, Eric and Chang, Angel X. and Savva, Manolis},
    title={{Habitat Synthetic Scenes Dataset (HSSD-200): An Analysis of 3D Scene Scale and Realism Tradeoffs for ObjectGoal Navigation}},
    journal={arXiv preprint},
    year={2023},
    eprint={2306.11290},
    archivePrefix={arXiv},
    primaryClass={cs.CV}
}
```
```bibtex
@article{Matterport3D,
  title={Matterport3D: Learning from RGB-D Data in Indoor Environments},
  author={Chang, Angel and Dai, Angela and Funkhouser, Thomas and Halber, Maciej and Niessner, Matthias and Savva, Manolis and Song, Shuran and Zeng, Andy and Zhang, Yinda},
  journal={International Conference on 3D Vision (3DV)},
  year={2017}
}
```
</details>