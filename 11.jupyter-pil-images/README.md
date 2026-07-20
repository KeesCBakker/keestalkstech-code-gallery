# Jupyter Notebook and PIL images

When working with images in a Python notebook I like to visualize them on a grid. Just calling
`display` is not enough as it renders the images underneath each other. This notebook uses
Matplotlib to generate a grid of PIL images with filenames as labels.

For more information check my blog: [Plotting a grid of PIL images in Jupyter](https://keestalkstech.com/plotting-a-grid-of-pil-images-in-jupyter/).

## Prerequisites

- Python 3.8+
- Jupyter Notebook

## Files

| File | Description |
|------|-------------|
| `plotting-a-grid-of-pil-images-in-jupyter.ipynb` | Jupyter notebook that downloads images from URLs and displays them in a grid |

## Dependencies

Install the required packages:

```sh
pip install pillow matplotlib
```

## Usage

```sh
jupyter notebook plotting-a-grid-of-pil-images-in-jupyter.ipynb
```

The notebook defines a `display_images` function with these parameters:

| Parameter | Default | Description |
|-----------|---------|-------------|
| `images` | required | List of PIL Image objects |
| `columns` | `5` | Number of columns in the grid |
| `width` | `20` | Figure width in inches |
| `height` | `8` | Figure height in inches |
| `max_images` | `15` | Maximum images to display |
| `label_wrap_length` | `50` | Max characters per filename line |
| `label_font_size` | `8` | Font size for image labels |

Images can be loaded from a URL using the included `url_to_pil` helper.

## Checkout only this project

```sh
git clone --no-checkout https://github.com/KeesCBakker/keestalkstech-code-gallery.git
cd keestalkstech-code-gallery
git sparse-checkout init
git sparse-checkout set --no-cone 11.jupyter-pil-images
git checkout main
cd 11.jupyter-pil-images
ls
```
