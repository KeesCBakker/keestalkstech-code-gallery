# Jupyter base with Python 3.11
ARG BASE_IMAGE=quay.io/jupyter/minimal-notebook:python-3.11
FROM ${BASE_IMAGE}

# Switch to root to install system packages
USER root

# Install Node.js 24.x via NodeSource
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl ca-certificates gnupg \
    && curl -fsSL https://deb.nodesource.com/setup_24.x | bash - \
    && apt-get install -y --no-install-recommends nodejs \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# (Optional) verify node/npm
RUN node -v && npm -v

# Switch back to the default notebook user (jovyan)
USER ${NB_UID}

# Default workdir for Jupyter stacks
WORKDIR /home/jovyan/work

# Install JS/TS Jupyter kernel (tslab)
# This registers both "jslab" (JavaScript) and "tslab" (TypeScript) kernels
RUN npm install -g tslab \
    && tslab install --sys-prefix

# Disable the announcements popup in JupyterLab
RUN jupyter labextension disable "@jupyterlab/apputils-extension:announcements"

