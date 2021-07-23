<template>
  <div
    class="pa-0 pa-sm-2 pb-sm-0"
    style="max-height: calc(100vh - 45px); min-height: calc(100vh - 75px)"
  >
    <template v-if="!landing">
      <template v-if="isPresentInProject">
        <div class="mt-sm-3">
          <v-breadcrumbs
            class="px-1 px-md-2 py-0 item-bread"
            :items="breadcrumbs"
          >
            <template v-slot:divider>
              <v-icon>mdi-chevron-right</v-icon>
            </template>
            <template v-slot:item="{ item }">
              <v-breadcrumbs-item :href="item.href" :disabled="item.disabled">
                {{ item.text }}
              </v-breadcrumbs-item>
            </template>
          </v-breadcrumbs>
        </div>
        <template v-if="!isMobile">
          <div class="d-flex align-center">
            <span class="px-1 px-md-2 page-header__title">{{
              duAn ? duAn.tenDuAn : ""
            }}</span>
            <v-divider
              vertical
              class="ml-1 mr-5 my-1 grey lighten-1"
            ></v-divider>
            <div class="d-flex align-center">
              <div
                v-for="member in dSThanhVienDuAn.slice(0, 5)"
                :key="member.id"
                class="avatar-group-member--item"
              >
                <v-tooltip top>
                  <template v-slot:activator="{ on }">
                    <router-link
                      :to="
                        member.hocVien
                          ? `/profile/${member.hocVien.account}`
                          : '/'
                      "
                    >
                      <v-avatar size="30" v-on="on">
                        <v-img
                          :src="
                            member &&
                            member.hocVien &&
                            member.hocVien.linkAnhCaNhan
                              ? HOST +
                                '/api/fileupload/download/' +
                                member.hocVien.linkAnhCaNhan
                              : '/static/img/default-avatar.png'
                          "
                        ></v-img>
                      </v-avatar>
                    </router-link>
                  </template>
                  <span>{{
                    member && member.hocVien ? member.hocVien.hoTen : "Hviter"
                  }}</span>
                </v-tooltip>
              </div>
              <v-btn
                v-if="dSThanhVienDuAn && dSThanhVienDuAn.length > 5"
                fab
                height="26"
                width="26"
                depressed
                class="ml-n2"
                dark
                style="z-index: 2; background-color: rgba(0, 0, 0, 0.5)"
              >
                +{{ dSThanhVienDuAn.length - 5 }}
              </v-btn>
              <v-chip
                small
                color="white"
                class="px-2 ml-1"
                @click="showModalThemNhanSu(duAn)"
                >+ Thêm</v-chip
              >
            </div>
            <v-spacer></v-spacer>
            <v-tooltip top>
              <template v-slot:activator="{ on, attrs }">
                <div v-bind="attrs" v-on="on" class="search-bar__wrapper">
                  <input
                    v-model="keywords"
                    @keyup.enter="getTrangThaiDuAn(filtersTrangThaiDuAn)"
                    type="text"
                    placeholder="Tìm kiếm"
                    class="search-bar__input"
                  />
                  <button type="button" class="search-bar__button">
                    <v-icon>search</v-icon>
                  </button>
                </div>
              </template>
              <span>Nhấn enter để tìm kiếm</span>
            </v-tooltip>
            <v-menu
              rounded="lg"
              bottom
              left
              offset-y
              :close-on-content-click="false"
              transition="scale-transition"
              origin="top right"
            >
              <template v-slot:activator="{ on: menu }">
                <v-tooltip top>
                  <template v-slot:activator="{ on: tooltip, attrs }">
                    <v-btn
                      depressed
                      outlined
                      fab
                      height="36"
                      width="36"
                      color="#738f93"
                      class="ml-2"
                      :loading="loading"
                      v-bind="attrs"
                      v-on="{ ...menu, ...tooltip }"
                    >
                      {{ monthSelected }}
                    </v-btn>
                  </template>
                  <span>Lọc theo tháng</span>
                </v-tooltip>
              </template>
              <v-card width="180">
                <v-card-title class="pb-2 pt-3 d-flex justify-center"
                  >Tháng
                </v-card-title>
                <v-card-text class="d-flex justify-space-between flex-wrap">
                  <v-btn
                    v-for="i in 12"
                    :key="i"
                    depressed
                    :outlined="i !== monthSelected"
                    dark
                    fab
                    height="36"
                    width="36"
                    color="#738f93"
                    class="mx-1 my-2"
                    :loading="loading"
                    @click="getTasksByMonth(i)"
                  >
                    {{ i }}
                  </v-btn>
                </v-card-text>
              </v-card>
            </v-menu>
            <v-tooltip top>
              <template v-slot:activator="{ on, attrs }">
                <v-btn
                  depressed
                  outlined
                  fab
                  height="36"
                  width="36"
                  color="#738f93"
                  class="ml-2"
                  :loading="loading"
                  v-bind="attrs"
                  v-on="on"
                  @click="
                    expandingAll
                      ? toggleExpandAll(false)
                      : toggleExpandAll(true)
                  "
                >
                  <v-icon size="20">{{
                    expandingAll ? "mdi-collapse-all" : "mdi-expand-all"
                  }}</v-icon>
                </v-btn>
              </template>
              <span>{{
                expandingAll ? "Thu gọn tất cả" : "Mở rộng tất cả"
              }}</span>
            </v-tooltip>
            <v-tooltip top>
              <template v-slot:activator="{ on, attrs }">
                <v-btn
                  depressed
                  outlined
                  fab
                  height="36"
                  width="36"
                  color="#738f93"
                  class="ml-2"
                  :loading="loading"
                  v-bind="attrs"
                  v-on="on"
                  @click="getTrangThaiDuAn(filtersTrangThaiDuAn)"
                >
                  <v-icon size="20">refresh</v-icon>
                </v-btn>
              </template>
              <span>Refresh</span>
            </v-tooltip>
            <v-tooltip top>
              <template v-slot:activator="{ on, attrs }">
                <v-btn
                  depressed
                  outlined
                  fab
                  height="36"
                  width="36"
                  color="#738f93"
                  class="ml-2"
                  :loading="loading"
                  v-bind="attrs"
                  v-on="on"
                  @click="isGettingMyTasks ? getTasksAllMember() : getMyTasks()"
                >
                  <v-icon size="20">{{
                    isGettingMyTasks ? "clear" : "mdi-account-details"
                  }}</v-icon>
                </v-btn>
              </template>
              <span>{{
                isGettingMyTasks ? "Bỏ lọc" : "Lọc công việc của tôi"
              }}</span>
            </v-tooltip>
            <v-tooltip top>
              <template v-slot:activator="{ on, attrs }">
                <v-btn
                  depressed
                  outlined
                  fab
                  height="36"
                  width="36"
                  color="#738f93"
                  class="ml-2"
                  :loading="loading"
                  v-bind="attrs"
                  v-on="on"
                  @click="
                    isGettingTasksToday ? getTasksInMonth() : getTasksToday()
                  "
                >
                  <v-icon size="20">{{
                    isGettingTasksToday ? "clear" : "mdi-calendar-check"
                  }}</v-icon>
                </v-btn>
              </template>
              <span>{{
                isGettingTasksToday ? "Bỏ lọc" : "Lọc công việc hôm nay"
              }}</span>
            </v-tooltip>
            <v-tooltip top>
              <template v-slot:activator="{ on, attrs }">
                <v-btn
                  depressed
                  outlined
                  fab
                  height="36"
                  width="36"
                  color="#738f93"
                  class="ml-2"
                  :loading="loading"
                  v-bind="attrs"
                  v-on="on"
                  @click="menuFilters = true"
                >
                  <v-icon size="20">mdi-filter-outline</v-icon>
                </v-btn>
              </template>
              <span>Lọc công việc</span>
            </v-tooltip>
            <template v-if="isAdminProject && dSTrangThaiDuAn.length != 0">
              <v-tooltip top>
                <template v-slot:activator="{ on, attrs }">
                  <v-btn
                    depressed
                    outlined
                    fab
                    height="36"
                    width="36"
                    color="#738f93"
                    class="ml-2"
                    :loading="loading"
                    :disabled="isProjectExpired"
                    v-bind="attrs"
                    v-on="on"
                    @click="showModalQuanLyCongViec(false, {}, duAnId)"
                  >
                    <v-icon size="22">add</v-icon>
                  </v-btn>
                </template>
                <span>{{
                  duAn && !isOverdue(duAn.thoiGianHetHan)
                    ? "Thêm mới"
                    : "Hết hạn"
                }}</span>
              </v-tooltip>
            </template>
          </div>
        </template>
        <template v-else>
          <div class="d-flex px-md-3 px-1 mt-2">
            <v-tooltip top>
              <template v-slot:activator="{ on, attrs }">
                <div v-on="on" v-bind="attrs" class="m-search-bar__wrapper">
                  <input
                    v-model="keywords"
                    @keyup.enter="getTrangThaiDuAn(filtersTrangThaiDuAn)"
                    type="text"
                    placeholder="Tìm kiếm"
                    class="m-search-bar__input"
                  />
                  <button type="button" class="m-search-bar__button">
                    <v-icon>search</v-icon>
                  </button>
                </div>
              </template>
              <span>Nhấn enter để tìm kiếm</span>
            </v-tooltip>
            <v-speed-dial direction="bottom" transition="slide-y-transition">
              <template v-slot:activator>
                <v-btn
                  depressed
                  outlined
                  fab
                  height="36"
                  width="36"
                  color="#738f93"
                  class="ml-2"
                  :loading="loading"
                  :disabled="isProjectExpired"
                >
                  <v-icon> mdi-menu-down-outline </v-icon>
                </v-btn>
              </template>
              <v-btn
                fab
                height="36"
                width="36"
                color="#738f93"
                dark
                :loading="loading"
                @click="showModalThemNhanSu(duAn)"
              >
                <v-icon size="22">mdi-account-outline</v-icon>
              </v-btn>
              <v-menu
                rounded="lg"
                bottom
                left
                offset-y
                :close-on-content-click="false"
                transition="scale-transition"
                origin="top right"
              >
                <template v-slot:activator="{ on }">
                  <v-btn
                    fab
                    height="36"
                    width="36"
                    color="#738f93"
                    dark
                    :loading="loading"
                    v-on="on"
                  >
                    {{ monthSelected }}
                  </v-btn>
                </template>
                <v-card width="180">
                  <v-card-title class="pb-2 pt-3 d-flex justify-center"
                    >Tháng
                  </v-card-title>
                  <v-card-text class="d-flex justify-space-between flex-wrap">
                    <v-btn
                      v-for="i in 12"
                      :key="i"
                      depressed
                      :outlined="i !== monthSelected"
                      dark
                      fab
                      height="36"
                      width="36"
                      color="#738f93"
                      class="mx-1 my-2"
                      :loading="loading"
                      @click="getTasksByMonth(i)"
                    >
                      {{ i }}
                    </v-btn>
                  </v-card-text>
                </v-card>
              </v-menu>
              <v-btn
                fab
                height="36"
                width="36"
                color="#738f93"
                dark
                :loading="loading"
                @click="
                  expandingAll ? toggleExpandAll(false) : toggleExpandAll(true)
                "
              >
                <v-icon size="20">{{
                  expandingAll ? "mdi-collapse-all" : "mdi-expand-all"
                }}</v-icon>
              </v-btn>
              <v-btn
                fab
                height="36"
                width="36"
                color="#738f93"
                dark
                :loading="loading"
                @click="getTrangThaiDuAn(filtersTrangThaiDuAn)"
              >
                <v-icon size="20">refresh</v-icon>
              </v-btn>
              <v-btn
                fab
                height="36"
                width="36"
                color="#738f93"
                dark
                :loading="loading"
                @click="isGettingMyTasks ? getTasksAllMember() : getMyTasks()"
              >
                <v-icon size="20">{{
                  isGettingMyTasks ? "clear" : "mdi-account-details"
                }}</v-icon>
              </v-btn>
              <v-btn
                fab
                height="36"
                width="36"
                color="#738f93"
                dark
                :loading="loading"
                @click="
                  isGettingTasksToday ? getTasksInMonth() : getTasksToday()
                "
              >
                <v-icon size="20">{{
                  isGettingTasksToday ? "clear" : "mdi-calendar-check"
                }}</v-icon>
              </v-btn>
              <v-btn
                fab
                height="36"
                width="36"
                color="#738f93"
                dark
                :loading="loading"
                @click="menuFilters = true"
              >
                <v-icon size="20">mdi-filter-outline</v-icon>
              </v-btn>
              <template v-if="isAdminProject && dSTrangThaiDuAn.length != 0">
                <v-btn
                  fab
                  height="36"
                  width="36"
                  color="#738f93"
                  dark
                  :loading="loading"
                  :disabled="isProjectExpired"
                  @click="showModalQuanLyCongViec(false, {}, duAnId)"
                >
                  <v-icon size="22">add</v-icon>
                </v-btn>
              </template>
            </v-speed-dial>
          </div>
        </template>
        <div class="d-flex mt-2" id="content">
          <div
            v-for="item in trangThaiCongViecs"
            :key="item.trangThaiId"
            class="mx-1"
          >
            <v-card
              width="272"
              class=""
              style="box-shadow: 0 1px 2px rgba(0, 0, 0, 0.2)"
            >
              <v-card-title
                class="tenTrangThai py-2 pl-2 pr-1"
                :style="`background-color: #50B5FF`"
              >
                <v-icon left color="white">mdi-format-list-bulleted</v-icon>
                {{ item.tenTrangThai ? item.tenTrangThai : "Trạng thái" }}
                <v-spacer></v-spacer>
                <v-chip
                  small
                  label
                  class="px-2"
                  style="
                    background-color: #e7f3ff;
                    color: #050505;
                    font-weight: 600;
                    margin-right: 6px;
                  "
                >
                  {{ item.totalCongViec }}
                </v-chip>
                <!-- <v-icon>mdi-dots-vertical</v-icon> -->
              </v-card-title>
              <!-- <v-divider class="grey lighten-2"></v-divider> -->
              <perfect-scrollbar class="inner-card-trang-thai--scroll">
                <div v-for="(member, iMember) in item.members" :key="iMember">
                  <v-divider
                    v-if="iMember !== 0"
                    class="grey-lighten-1"
                  ></v-divider>
                  <v-card flat tile class="card-member">
                    <div class="d-flex py-2 pl-3 pr-2">
                      <span
                        >{{ member.info ? member.info.hoTen : "Unknown" }}
                        {{
                          member.congViecs.length > 0
                            ? `(${member.congViecs.length})`
                            : ""
                        }}</span
                      >
                      <v-spacer></v-spacer>
                      <v-icon
                        v-if="member.congViecs && member.congViecs.length > 0"
                        @click="member.showCongViec = !member.showCongViec"
                        >{{
                          member.showCongViec
                            ? "mdi-chevron-down"
                            : "mdi-chevron-right"
                        }}</v-icon
                      >
                    </div>
                  </v-card>
                  <v-divider
                    v-if="
                      item.members &&
                      iMember !== item.members.length &&
                      member.showCongViec &&
                      member.congViecs &&
                      member.congViecs.length > 0
                    "
                    class="grey-lighten-1"
                  ></v-divider>
                  <v-expand-transition>
                    <template v-if="member.showCongViec">
                      <dragable
                        v-model="member.congViecs"
                        :disabled="
                          isProjectExpired || !isPresentInProject || isMobile
                        "
                        v-bind="dragOptions"
                        :move="checkMove"
                        @add="onAdd(item.trangThaiId)"
                        class="drag"
                      >
                        <v-card
                          v-for="(congViec, index) in member.congViecs"
                          :key="index"
                          :ripple="false"
                          class="
                            my-2
                            mr-2
                            pa-2
                            pr-1
                            rounded-l-0
                            q-card-shadow
                            rounded-lg
                            card-dnd-item
                          "
                          :style="`border-left: 3px solid ${getColorPriority(
                            congViec
                          )} !important;
                                  box-shadow: 0 1px 3px 0 rgba(0,0,0,.16), 0 0 0 1px rgba(0,0,0,.08) !important;`"
                          title="Click 2 lần để sửa"
                          @dblclick="
                            showModalQuanLyCongViec(true, congViec, duAnId)
                          "
                        >
                          <v-card-title class="pa-0 mb-1">
                            <v-chip
                              small
                              color="#E7F3FF"
                              style="padding-left: 10px; padding-right: 10px"
                            >
                              <span style="color: #1876f2">{{
                                congViec.loaiCongViec
                                  ? congViec.loaiCongViec.tenLoaiCongViec
                                  : "Work"
                              }}</span>
                            </v-chip>
                            <v-spacer></v-spacer>
                            <v-btn
                              v-if="
                                congViec.lienHe ||
                                getPhoneNumber(congViec.tenCongViec)
                              "
                              icon
                              small
                              class="mt-n1"
                              color="grey"
                              :href="`tel:${
                                congViec.lienHe
                                  ? congViec.lienHe
                                  : getPhoneNumber(congViec.tenCongViec)
                              }`"
                              ><v-icon size="18">mdi-phone</v-icon>
                            </v-btn>
                            <v-menu
                              transition="slide-y-transition"
                              v-if="isAdminProject"
                              class="pb-5 pr-2"
                              offset-y
                              bottom
                              left
                              rounded="lg"
                              :close-on-content-click="false"
                            >
                              <template v-slot:activator="{ on, attrs }">
                                <v-btn
                                  v-bind="attrs"
                                  v-on="on"
                                  icon
                                  small
                                  class="mt-n1"
                                  color="grey"
                                  ><v-icon>mdi-dots-horizontal</v-icon>
                                </v-btn>
                              </template>
                              <v-list>
                                <v-list-item
                                  @click="
                                    showModalQuanLyCongViec(
                                      true,
                                      congViec,
                                      duAnId
                                    )
                                  "
                                >
                                  <v-list-item-title>{{
                                    isProjectExpired || !isAdminProject
                                      ? "Xem"
                                      : "Cập nhật"
                                  }}</v-list-item-title>
                                </v-list-item>
                                <v-list-item @click="confirmDelete(congViec)">
                                  <v-list-item-title>Xóa</v-list-item-title>
                                </v-list-item>
                              </v-list>
                            </v-menu>
                          </v-card-title>
                          <div class="pb-2 ten-cong-viec">
                            {{ congViec ? congViec.tenCongViec : "" }}
                          </div>
                          <div class="d-flex align-center">
                            <v-tooltip top>
                              <template v-slot:activator="{ on, attrs }">
                                <v-chip
                                  small
                                  label
                                  dark
                                  v-bind="attrs"
                                  v-on="on"
                                  class="pr-2 py-1"
                                  :color="getStateCongViec(congViec).color"
                                >
                                  <v-icon size="16" left>{{
                                    getStateCongViec(congViec).icon
                                  }}</v-icon>
                                  {{ formatDate(congViec.ngayHetHan) }}
                                </v-chip>
                              </template>
                              <span>{{
                                getStateCongViec(congViec).content
                              }}</span>
                            </v-tooltip>
                            <v-chip
                              v-if="congViec.soLuongChiTietCongViec != 0"
                              small
                              label
                              dark
                              class="pr-2 py-1 mx-2"
                              color="#61BD4F"
                            >
                              <v-icon size="16" left
                                >mdi-checkbox-marked-outline</v-icon
                              >
                              {{
                                congViec.soLuongChiTietCongViecHoanThanh +
                                "/" +
                                congViec.soLuongChiTietCongViec
                              }}
                            </v-chip>
                            <v-spacer></v-spacer>
                            <div class="mr-1 d-flex align-center">
                              <div
                                v-for="(
                                  nguoiLam, idxPV
                                ) in congViec.phanViec.slice(0, 3)"
                                :key="idxPV"
                                class="avatar-group-member--item"
                              >
                                <v-tooltip top>
                                  <template v-slot:activator="{ on }">
                                    <v-avatar size="24" v-on="on">
                                      <v-img
                                        :src="
                                          nguoiLam.thanhVienDuAn &&
                                          nguoiLam.thanhVienDuAn.hocVien &&
                                          nguoiLam.thanhVienDuAn.hocVien
                                            .linkAnhCaNhan
                                            ? HOST +
                                              '/api/fileupload/download/' +
                                              nguoiLam.thanhVienDuAn.hocVien
                                                .linkAnhCaNhan
                                            : '/static/img/default-avatar.png'
                                        "
                                        class="grey lighten-2"
                                      ></v-img>
                                    </v-avatar>
                                  </template>
                                  <span>{{
                                    nguoiLam.thanhVienDuAn &&
                                    nguoiLam.thanhVienDuAn.hocVien
                                      ? nguoiLam.thanhVienDuAn.hocVien.hoTen
                                      : "Hviter"
                                  }}</span>
                                </v-tooltip>
                              </div>
                              <v-btn
                                v-if="
                                  congViec.phanViec &&
                                  congViec.phanViec.length > 3
                                "
                                fab
                                height="24"
                                width="24"
                                depressed
                                class="ml-n2"
                                dark
                                style="
                                  z-index: 2;
                                  font-size: 12px;
                                  background-color: rgba(0, 0, 0, 0.5);
                                "
                              >
                                +{{ congViec.phanViec.length - 3 }}
                              </v-btn>
                            </div>
                          </div>
                          <div
                            v-if="
                              congViec.tagCongViec &&
                              congViec.tagCongViec.length !== 0
                            "
                            class="mt-2"
                            style="font-size: 14px"
                          >
                            <span
                              v-for="(tagCongViec, i) in congViec.tagCongViec"
                              :key="i"
                              style="color: #1876f2"
                            >
                              #{{
                                tagCongViec && tagCongViec.tag
                                  ? tagCongViec.tag.tenTag + " "
                                  : " "
                              }}
                            </span>
                          </div>
                        </v-card>
                      </dragable>
                    </template>
                  </v-expand-transition>
                </div>
              </perfect-scrollbar>
            </v-card>
          </div>
        </div>
        <v-navigation-drawer
          id="appDrawer"
          v-model="menuFilters"
          width="300"
          floating
          right
          app
          fixed
          clipped
        >
          <perfect-scrollbar class="filter--scroll-bar">
            <v-card flat>
              <q-card-title
                :title="`Menu lọc`"
                @click="menuFilters = false"
              ></q-card-title>
              <v-card-text>
                <v-row class="wrapper-filters">
                  <v-col cols="12" class="mb-2 py-0">
                    <span class="q-input-label">Loại công việc</span>
                    <v-autocomplete
                      v-model="filtersTrangThaiDuAn.loaiCongViecId"
                      hide-details
                      solo
                      flat
                      dense
                      :items="dSLoaiCongViec"
                      item-value="id"
                      item-text="tenLoaiCongViec"
                      @input="getTrangThaiDuAn(filtersTrangThaiDuAn)"
                      persistent-hint
                      class="mt-0"
                      :loading="loading"
                    ></v-autocomplete>
                  </v-col>
                  <v-col cols="12" class="mb-2 py-0">
                    <span class="q-input-label">Tag</span>
                    <v-autocomplete
                      v-model="filtersTrangThaiDuAn.tagId"
                      hide-details
                      solo
                      flat
                      dense
                      :items="dsTag"
                      item-value="id"
                      item-text="tenTag"
                      @input="getTrangThaiDuAn(filtersTrangThaiDuAn)"
                      persistent-hint
                      class="mt-0"
                      :loading="loading"
                    ></v-autocomplete>
                  </v-col>
                  <v-col cols="12" class="mb-2 py-0">
                    <span class="q-input-label">Người làm</span>
                    <v-autocomplete
                      v-model="filtersTrangThaiDuAn.nguoiLamId"
                      hide-details
                      solo
                      flat
                      dense
                      :items="dSThanhVienDuAn"
                      item-value="hocVienId"
                      item-text="hocVien.hoTen"
                      @input="getTrangThaiDuAn(filtersTrangThaiDuAn)"
                      persistent-hint
                      class="mt-0"
                      :loading="loading"
                    ></v-autocomplete>
                  </v-col>
                  <v-col cols="12" class="mb-2 py-0">
                    <span class="q-input-label">Từ ngày</span>
                    <v-datepicker
                      hide-details
                      solo
                      flat
                      dense
                      v-model="filtersTrangThaiDuAn.tuNgay"
                      @input="getTrangThaiDuAn(filtersTrangThaiDuAn)"
                      class="datepicker-input"
                    ></v-datepicker>
                  </v-col>
                  <v-col cols="12" class="mb-2 py-0">
                    <span class="q-input-label">Đến ngày</span>
                    <v-datepicker
                      hide-details
                      solo
                      flat
                      dense
                      v-model="filtersTrangThaiDuAn.denNgay"
                      @input="getTrangThaiDuAn(filtersTrangThaiDuAn)"
                      class="datepicker-input"
                    ></v-datepicker>
                  </v-col>
                </v-row>
              </v-card-text>
            </v-card>
          </perfect-scrollbar>
        </v-navigation-drawer>
      </template>
      <template v-else>
        <v-card
          class="
            d-flex
            align-center
            q-card-shadow
            rounded-lg
            card-min-height
            mt-sm-3
          "
        >
          <v-card-text
            class="d-flex justify-center font-weight-bold"
            style="color: #636363; font-size: 24px"
          >
            Bạn không phải thành viên.
          </v-card-text>
        </v-card>
      </template>
    </template>
    <template v-else>
      <div class="mt-sm-3">
        <circle-loading></circle-loading>
      </div>
    </template>
    <modal-quan-ly-cong-viec
      ref="modalQuanLyCongViec"
      @updated="getTrangThaiDuAn(filtersTrangThaiDuAn)"
    ></modal-quan-ly-cong-viec>
    <modal-them-sua-nhan-su
      ref="modalThemSuaNhanSu"
      @updatedQLCV="getThanhVienDuAn(filtersThanhVienDuAn)"
    ></modal-them-sua-nhan-su>
    <v-dialog v-model="dialogConfirmDelete" max-width="290">
      <v-card>
        <v-card-title class="headline">Xác nhận xóa</v-card-title>
        <v-card-text class="pt-3">Bạn có chắc chắn muốn xóa?</v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn @click.native="dialogConfirmDelete = false" text>Hủy</v-btn>
          <v-btn
            :loading="loading"
            color="red darken-1"
            text
            @click="deleteCongViec"
            >Xác Nhận</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script lang="ts">
import ModalQuanLyCongViec from "./ModalQuanLyCongViec.vue";
import { Vue } from "vue-property-decorator";
import { LoaiCongViecDTO } from "@/models/TaskManagement/LoaiCongViecDTO";
import LoaiCongViecApi, {
  GetLoaiCongViecParams,
} from "../../apiResources/TaskManagement/LoaiCongViecApi";
import { TrangThaiDuAnDTO } from "@/models/TaskManagement/TrangThaiDuAnDTO";
import TrangThaiDuAnApi, {
  GetTrangThaiDuAnParams,
} from "../../apiResources/TaskManagement/TrangThaiDuAnApi";
import { CongViecDTO } from "@/models/TaskManagement/CongViecDTO";
import CongViecApi, {
  GetCongViecParams,
} from "../../apiResources/TaskManagement/CongViecApi";
import { DuAnDTO } from "@/models/TaskManagement/DuAnDTO";
import DuAnApi, {
  GetDuAnParams,
} from "../../apiResources/TaskManagement/DuAnApi";
import { ThanhVienDuAnDTO } from "@/models/TaskManagement/ThanhVienDuAnDTO";
import ThanhVienDuAnApi, {
  GetThanhVienDuAnParams,
} from "../../apiResources/TaskManagement/ThanhVienDuAnApi";
import { PhanViecDTO } from "@/models/TaskManagement/PhanViecDTO";
import PhanViecApi, {
  GetPhanViecParams,
} from "../../apiResources/TaskManagement/PhanViecApi";
import dragable from "vuedraggable";
import ModalThemSuaNhanSu from "./ModalThemSuaNhanSu.vue";
import { HocVienDTO } from "@/models/HocVienDTO";
import { TagDTO } from "@/models/TaskManagement/TagDTO";
import TagApi, { GetTagParams } from "../../apiResources/TaskManagement/TagApi";
import CircleLoading from "../Commons/LoadingTemplate/CircleLoading.vue";

export default Vue.extend({
  components: {
    dragable,
    ModalQuanLyCongViec,
    ModalThemSuaNhanSu,
    CircleLoading,
  },
  data() {
    return {
      keywords: null as any,
      drag: false,
      menuFilters: false,
      landing: false,
      loading: false,
      dSLoaiCongViec: [] as LoaiCongViecDTO[],
      duAn: {} as DuAnDTO,
      duAnId: 0 as number,
      dSTrangThaiDuAn: [] as TrangThaiDuAnDTO[],
      filtersTrangThaiDuAn: {
        itemsPerPage: -1,
        tagId: null as any,
        nguoiLamId: null as any,
        loaiCongViecId: null as any,
      } as GetTrangThaiDuAnParams,
      dSCongViec: [] as CongViecDTO[],
      dSThanhVienDuAn: [] as ThanhVienDuAnDTO[],
      filtersThanhVienDuAn: { itemsPerPage: 25 } as GetThanhVienDuAnParams,
      totalItemsTVDA: 0 as number | undefined,
      filtersPhanViec: { itemsPerPage: 25 } as GetPhanViecParams,
      dSPhanViec: [] as PhanViecDTO[],
      totalItemsPV: 0 as number | undefined,
      dialogConfirmDelete: false,
      congViecSelected: {} as CongViecDTO,
      congViecChanged: {} as CongViecDTO,
      today: null as any,
      hocVien: {} as HocVienDTO,
      HOST: process.env.VUE_APP_ROOT_API,
      thanhVienDuAnOfUser: [] as ThanhVienDuAnDTO[],
      dsTag: [] as TagDTO[],
      isGettingTasksToday: false,
      isGettingMyTasks: false,
      regex: /([\+84|84|0]+(3|5|7|8|9|1[2|6|8|9]))+([0-9]{8})/g,
      trangThaiCongViecs: [] as any[],
      expandingAll: true,
      monthSelected: this.$moment().month() + 1,
    };
  },
  created() {
    // this.updateDrawer();
    this.landing = true;
    this.today = this.$moment().format("YYYY-MM-DD");
    this.getDSLoaiCongViec();
    this.duAnId = parseInt(this.$route.params.nhomViecId);
    this.getDuAnById(this.duAnId);
    this.filtersTrangThaiDuAn.duAnId = this.duAnId;
    this.getTasksInMonth();
    this.filtersThanhVienDuAn.duAnId = this.duAnId;
    this.getThanhVienDuAn(this.filtersThanhVienDuAn);
    this.getDSTag();
  },
  mounted() {},
  computed: {
    currentRoles() {
      return this.$store.state.user.Profile.UserRoles;
    },
    dragOptions() {
      return {
        animation: 200,
        group: "list",
        disabled: false,
        ghostClass: "ghost",
      };
    },
    breadcrumbs(): any {
      return [
        {
          text: "Nhóm việc",
          disabled: false,
          href: "#/congviec/quanlynhomviec",
        },
        {
          text: this.duAn ? this.duAn.tenDuAn : "",
          disabled: true,
          href: "#/",
        },
      ];
    },
    isMobile(): any {
      return this.$vuetify.breakpoint.smAndDown;
    },
    myProfile(): any {
      return this.$store.state.user.Profile;
    },
    isPresentInProject(): any {
      return (
        this.dSThanhVienDuAn.findIndex(
          (x) => x.hocVienId === this.myProfile.UserId
        ) !== -1
      );
    },
    myRoleOfProject(): any {
      let role = this.dSThanhVienDuAn.find(
        (x) =>
          x.hocVienId === this.myProfile.UserId &&
          x.quyenDuAn.duAnId == this.duAnId
      );
      if (role) return role.quyenDuAn.quyen.tenQuyen;
      else return "Guest";
    },
    isAdminProject(): any {
      return this.myRoleOfProject === "Admin";
    },
    isProjectExpired(): any {
      return (
        this.duAn &&
        this.duAn.thoiGianHetHan &&
        this.isOverdue(this.duAn.thoiGianHetHan)
      );
    },
  },
  watch: {
    keywords: function (newVal) {
      if (newVal === null || newVal === undefined || newVal === "") {
        this.keywords = null;
        this.getTrangThaiDuAn(this.filtersTrangThaiDuAn);
      }
    },
    menuFilters: function (newVal) {
      if (newVal && this.$store.state.app.drawer) {
        this.updateDrawer();
      }
    },
  },
  methods: {
    // getColorTrangThai() {
    //   let lstColor = [
    //     "#50B5FF",
    //     "#FF9B8A",
    //     "#FFBA68",
    //     "#D592FF",
    //     "#F06292",
    //     "#7986CB",
    //     "#4FC3F7",
    //     "#1DE9B6",
    //   ];
    //   const random = Math.floor(Math.random() * lstColor.length);
    //   return lstColor[random];
    // },
    getPhoneNumber(text: string) {
      let phoneNumber = text.match(this.regex);
      return phoneNumber ? phoneNumber[0] : null;
    },
    updateDrawer() {
      let app = this.$store.state.app;
      app.drawer = !this.$store.state.app.drawer;
      this.$store.commit("UPDATE_APP", app);
    },
    getColorPriority(congViec: CongViecDTO) {
      if (congViec) {
        if (congViec.doUuTien === "cao") return "#FF9B8A";
        else if (congViec.doUuTien === "rất cao") return "#D592FF";
        else if (congViec.doUuTien === "bình thường") return "#1ee58e";
        else return "#50B5FF";
      }
    },
    getDSLoaiCongViec() {
      LoaiCongViecApi.getLoaiCongViec({ itemsPerPage: -1 })
        .then((response) => {
          this.dSLoaiCongViec = response.data;
          this.dSLoaiCongViec.unshift({
            id: null as any,
            tenLoaiCongViec: "Chọn tất cả",
          });
        })
        .catch((err) => {
          this.$snotify.error("Lây dữ liệu loại công việc thất bại!");
        });
    },
    getDSTag() {
      TagApi.getTag({ itemsPerPage: -1 })
        .then((response) => {
          this.dsTag = response.data;
          this.dsTag.unshift({
            id: null as any,
            tenTag: "Chọn tất cả",
          });
        })
        .catch((err) => {
          this.$snotify.error("Lây dữ liệu tag công việc thất bại!");
        });
    },
    getTasksToday() {
      this.isGettingTasksToday = true;
      this.filtersTrangThaiDuAn.tuNgay = this.$moment().format("YYYY-MM-DD");
      this.filtersTrangThaiDuAn.denNgay = this.$moment().format("YYYY-MM-DD");
      this.getTrangThaiDuAn(this.filtersTrangThaiDuAn);
    },
    getTasksInMonth() {
      this.isGettingTasksToday = false;
      this.filtersTrangThaiDuAn.tuNgay = this.getFirstDay();
      this.filtersTrangThaiDuAn.denNgay = this.getEndDay();
      this.getTrangThaiDuAn(this.filtersTrangThaiDuAn);
    },
    getMyTasks() {
      this.isGettingMyTasks = true;
      this.filtersTrangThaiDuAn.nguoiLamId = this.myProfile.UserId;
      this.getTrangThaiDuAn(this.filtersTrangThaiDuAn);
    },
    getTasksAllMember() {
      this.isGettingMyTasks = false;
      this.filtersTrangThaiDuAn.nguoiLamId = null as any;
      this.getTrangThaiDuAn(this.filtersTrangThaiDuAn);
    },
    getTrangThaiDuAn(filters: GetTrangThaiDuAnParams) {
      this.loading = true;
      filters.tenCongViec = this.keywords;
      TrangThaiDuAnApi.getTrangThaiDuAnByDuAnId(filters)
        .then((res) => {
          this.dSTrangThaiDuAn = res.data;
          this.handleDanhSachTrangThaiDuAn(res.data);
          this.landing = false;
          this.loading = false;
        })
        .catch((err) => {
          this.landing = false;
          this.loading = false;
          this.$snotify.error("Lây dữ liệu trạng thái dự án thất bại!");
        });
    },
    handleDanhSachTrangThaiDuAn(trangThaiDuAns: TrangThaiDuAnDTO[]) {
      this.trangThaiCongViecs = [];
      trangThaiDuAns
        .map((x) => x.trangThai)
        .forEach((x) => {
          let trangThai = {
            trangThaiId: x.id,
            tenTrangThai: x.tenTrangThai,
            totalCongViec: x.congViec.length,
            members: [] as any[],
          };
          if (x.congViec && x.congViec.length > 0) {
            let currentMemberInfo = this.getInfoMemberByCongViec(x.congViec[0]);
            if (currentMemberInfo) {
              let currentMember = {
                showCongViec: true,
                info: currentMemberInfo,
                congViecs: this.getCongViecsByMember(
                  x.congViec,
                  currentMemberInfo
                ),
              };
              trangThai.members.push(currentMember);
              for (let i = 1; i < x.congViec.length; i++) {
                let otherMemberInfo = this.getInfoMemberByCongViec(
                  x.congViec[i]
                ) as HocVienDTO;
                if (
                  otherMemberInfo.id !== currentMember.info.id &&
                  trangThai.members.findIndex(
                    (m) => m.info.id === otherMemberInfo.id
                  ) === -1
                ) {
                  let otherMember = {
                    showCongViec: true,
                    info: otherMemberInfo,
                    congViecs: this.getCongViecsByMember(
                      x.congViec,
                      otherMemberInfo
                    ),
                  };
                  trangThai.members.push(otherMember);
                  currentMember = otherMember;
                }
              }
            }
            this.trangThaiCongViecs.push(trangThai);
          } else {
            let noneMember = {
              showCongViec: true,
              info: {
                id: null,
                hoTen: "Chưa có công việc",
              },
              congViecs: [],
            };
            trangThai.members.push(noneMember);
            this.trangThaiCongViecs.push(trangThai);
          }
        });
    },
    getInfoMemberByCongViec(congViec: CongViecDTO) {
      return congViec.phanViec &&
        congViec.phanViec.length > 0 &&
        congViec.phanViec[0].thanhVienDuAn &&
        congViec.phanViec[0].thanhVienDuAn.hocVien
        ? congViec.phanViec[0].thanhVienDuAn.hocVien
        : ({
            id: 0,
            hoTen: "Unknown",
          } as HocVienDTO);
    },
    getCongViecsByMember(congViecs: CongViecDTO[], member: HocVienDTO) {
      if (member.id !== 0) {
        return congViecs.filter(
          (cv) =>
            cv.phanViec &&
            cv.phanViec.length > 0 &&
            cv.phanViec[0].thanhVienDuAn &&
            cv.phanViec[0].thanhVienDuAn.hocVienId === member.id
        );
      } else
        return congViecs.filter(
          (cv) => cv.phanViec && cv.phanViec.length === 0
        );
    },
    getThanhVienDuAn(filters: GetThanhVienDuAnParams) {
      ThanhVienDuAnApi.getThanhVienDuAn(filters)
        .then((response) => {
          this.dSThanhVienDuAn = response.data;
        })
        .catch((err) => {
          this.$snotify.error("Lây dữ liệu thành viên dự án thất bại!");
        });
    },
    getDuAnById(duAnId: number) {
      DuAnApi.getDuAnById(duAnId)
        .then((res) => {
          this.duAn = res;
        })
        .catch((res) => {
          this.$snotify.error("Lấy thông tin dự án thất bại!");
        });
    },
    formatDate(date: Date) {
      if (!date) {
        return "Không xác định";
      } else return this.$moment(date).format("DD/MM/YYYY");
    },
    confirmDelete(congViec: any): void {
      this.dialogConfirmDelete = true;
      this.congViecSelected = congViec;
    },
    deleteCongViec(): void {
      CongViecApi.deleteCongViec(this.congViecSelected.id)
        .then((res) => {
          this.dialogConfirmDelete = false;
          this.getTrangThaiDuAn(this.filtersTrangThaiDuAn);
        })
        .catch((res) => {
          this.$snotify.error("Xóa thất bại!");
        });
    },
    updateCongViec() {
      this.loading = true;
      CongViecApi.updateCongViec(this.congViecChanged.id, this.congViecChanged)
        .then((res) => {
          this.getTrangThaiDuAn(this.filtersTrangThaiDuAn);
          this.loading = false;
        })
        .catch((res) => {
          this.loading = false;
          this.$snotify.error("Cập nhật thất bại!");
        });
    },
    showModalQuanLyCongViec(isUpdate: boolean, item: any, duAnId: number) {
      (this.$refs.modalQuanLyCongViec as any).show(isUpdate, item, duAnId);
    },
    showModalThemNhanSu(item: any) {
      (this.$refs.modalThemSuaNhanSu as any).show(item);
    },
    toggleExpandAll(value: boolean) {
      this.expandingAll = value;
      this.trangThaiCongViecs = this.trangThaiCongViecs.map((x: any) => {
        x.members = x.members.map((y: any) => {
          if (y.info && y.info.id !== null) {
            y.showCongViec = value;
          }
          return y;
        });
        return x;
      });
    },
    getTasksByMonth(month: any) {
      this.monthSelected = month;
      this.filtersTrangThaiDuAn.tuNgay = this.$moment()
        .month(month - 1)
        .startOf("month")
        .format("YYYY-MM-DD");
      this.filtersTrangThaiDuAn.denNgay = this.$moment()
        .month(month - 1)
        .endOf("month")
        .format("YYYY-MM-DD");
      this.getTrangThaiDuAn(this.filtersTrangThaiDuAn);
    },
    //code dragable
    checkMove(evt: any) {
      this.congViecChanged = evt.draggedContext.element;
    },
    onAdd(trangThaiId: number) {
      this.$set(this.congViecChanged, "trangThaiId", trangThaiId);
      this.updateCongViec();
    },
    //end code dragable
    getStateCongViec(congViec: CongViecDTO) {
      if (congViec.isActive) {
        if (this.isOverdue(congViec.ngayHetHan))
          return {
            id: 1,
            color: "#FF9B8A",
            icon: "mdi-clock-outline",
            content: "Đã hết hạn",
          };
        else
          return {
            id: 2,
            color: "#50B5FF",
            icon: "mdi-clock-outline",
            content: "",
          };
      } else
        return {
          id: 3,
          color: "#FFBA68",
          icon: "mdi-lock",
          content: "Đã khóa",
        };
    },
    isOverdue(date: Date) {
      return this.$moment(this.today).isAfter(date, "day");
    },
    getFirstDay() {
      return this.$moment().clone().startOf("month").format("YYYY-MM-DD");
    },
    getEndDay() {
      return this.$moment().clone().endOf("month").format("YYYY-MM-DD");
    },
    // add(event: any) {
    //   event.target.disabled = true;
    // },
  },
});
</script>
<style scoped>
/* Search bar */
.search-bar__wrapper {
  display: flex;
  width: 200px;
  height: 36px;
  border-radius: 50px;
  border: 1px solid #738f93;
  transition: all 0.2s ease-in-out;
}
.search-bar__wrapper:focus-within {
  width: 240px;
  background-color: #fff;
}
.search-bar__input {
  display: flex;
  align-self: center;
  font-size: 14px;
  height: 36px;
  padding: 0 12px;
  background: transparent;
  outline: 0;
  width: 100%;
}
.search-bar__button {
  margin-left: auto;
  margin-right: 8px;
}
.m-search-bar__wrapper {
  display: flex;
  width: 100%;
  height: 36px;
  border-radius: 50px;
  border: 1px solid #738f93;
  transition: all 0.2s ease-in-out;
}
.m-search-bar__wrapper:focus-within {
  background-color: #fff;
}
.m-search-bar__input {
  display: flex;
  align-self: center;
  font-size: 14px;
  height: 36px;
  padding: 0 12px;
  background: transparent;
  outline: 0;
  width: 100%;
}
.m-search-bar__button {
  margin-left: auto;
  margin-right: 8px;
}
#content {
  overflow-x: auto;
  overflow-y: hidden;
  /* min-height: calc(100vh - 165px); */
  height: calc(100vh - 160px);
  max-height: calc(100vh - 160px);
}
.inner-card-trang-thai--scroll {
  max-height: calc(100vh - 230px);
}
.tenTrangThai {
  color: white;
  font-size: 16px;
}
.ten-cong-viec {
  color: #172b4d;
  font-size: 14px;
  letter-spacing: 0.2px;
  word-break: break-word;
}
.card-dnd-item:hover {
  cursor: pointer;
}
.filter--scroll-bar {
  height: calc(100% - 60px);
  overflow: auto;
}
/* .card-member:not(:last-child) {
  border-top: 1px solid #ddd;
  border-bottom: 1px solid #ddd;
} */
</style>
