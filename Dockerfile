# syntax=docker/dockerfile:1

FROM ghcr.io/linuxserver/baseimage-kasmvnc:ubuntujammy

# set version label
ARG BUILD_DATE
ARG VERSION
ARG LIBATION_RELEASE
LABEL build_version="Linuxserver.io version:- ${VERSION} Build-date:- ${BUILD_DATE}"
LABEL maintainer="cyndane31"

ENV \
  CUSTOM_PORT="30303" \
  CUSTOM_HTTPS_PORT="31313" \
  HOME="/config" \
  TITLE="Libation"

RUN \
  echo "**** install runtime packages ****" && \
  apt-get update && \
  apt-get install -y --no-install-recommends \
    dbus \
    fcitx-rime \
    fonts-wqy-microhei \
    libnss3 \
    libopengl0 \
    libqpdf28 \
    libxkbcommon-x11-0 \
    libxcb-icccm4 \
    libxcb-image0 \
    libxcb-keysyms1 \
    libxcb-randr0 \
    libxcb-render-util0 \
    libxcb-xinerama0 \
    poppler-utils \
    python3 \
    python3-xdg \
    ttf-wqy-zenhei \
    wget \
    xz-utils && \
  apt-get install -y speech-dispatcher gedit
RUN \
  echo "**** install libation ****" && \
  mkdir -p /opt/libation && \
  if [ -z ${LIBATION_RELEASE+x} ]; then \
    LIBATION_RELEASE=$(curl -sX GET "https://api.github.com/repos/rmcrackan/Libation/releases/latest" \
    | jq -r .tag_name); \
  fi && \
  LIBATION_VERSION="$(echo ${LIBATION_RELEASE} | cut -c2-)" && \
  wget -O libation.deb https://github.com/rmcrackan/Libation/releases/download/"$LIBATION_RELEASE"/Libation."$LIBATION_VERSION"-linux-chardonnay-amd64.deb && \
  apt-get install ./libation.deb && \
  dbus-uuidgen > /etc/machine-id && \
  sed -i 's|</applications>|  <application title="libation" type="normal">\n    <maximized>yes</maximized>\n  </application>\n</applications>|' /etc/xdg/openbox/rc.xml && \
  echo "**** cleanup ****" && \
  apt-get clean && \
  rm -rf \
    /tmp/* \
    /var/lib/apt/lists/* \
    /var/tmp/*

# add local files
COPY root/ /