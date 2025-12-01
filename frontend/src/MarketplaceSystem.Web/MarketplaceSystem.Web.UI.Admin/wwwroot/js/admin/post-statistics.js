(() => {
    const config = window.POST_STATISTICS_CONFIG || {};
    if (!config.summaryEndpoint) {
        console.warn('Không tìm thấy endpoint thống kê bài đăng.');
        return;
    }

    const elements = {
        form: document.getElementById('postStatisticsFilter'),
        startInput: document.getElementById('postStartDate'),
        endInput: document.getElementById('postEndDate'),
        quickRange: document.getElementById('postQuickRange'),
        categoryInput: document.getElementById('postCategoryFilter'),
        loading: document.getElementById('postStatisticsLoading'),
        error: document.getElementById('postStatisticsError'),
        meta: document.getElementById('postStatisticsMeta'),
        totalPostsCount: document.getElementById('totalPostsCount'),
        totalPostsGrowth: document.getElementById('totalPostsGrowth'),
        approvedPostsCount: document.getElementById('approvedPostsCount'),
        approvedPostsPercent: document.getElementById('approvedPostsPercent'),
        pendingPostsCount: document.getElementById('pendingPostsCount'),
        pendingPostsPercent: document.getElementById('pendingPostsPercent'),
        rejectedPostsCount: document.getElementById('rejectedPostsCount'),
        rejectedPostsPercent: document.getElementById('rejectedPostsPercent'),
        averageViewsCurrent: document.getElementById('averageViewsCurrent'),
        averageViewsChange: document.getElementById('averageViewsChange'),
        timelineMode: document.getElementById('postTimelineMode'),
        timelineNotes: document.getElementById('postTimelineNotes'),
        statusApprovedCount: document.getElementById('statusApprovedCount'),
        statusPendingCount: document.getElementById('statusPendingCount'),
        statusRejectedCount: document.getElementById('statusRejectedCount'),
        parentCategoryList: document.getElementById('parentCategoryList'),
        topCategoriesBody: document.getElementById('topCategoriesBody'),
        exportBtn: document.getElementById('exportPostStats'),
        printBtn: document.getElementById('printPostStats')
    };

    const chartContexts = {
        trend: document.getElementById('postsTrendChart')?.getContext('2d'),
        status: document.getElementById('postStatusChart')?.getContext('2d'),
        parentCategory: document.getElementById('parentCategoryChart')?.getContext('2d')
    };

    const charts = {
        trend: null,
        status: null,
        parentCategory: null
    };

    const MAX_DAILY_POINTS = 60;
    let latestSummary = null;
    let currentRange = {
        from: elements.startInput?.value || config.defaultFrom,
        to: elements.endInput?.value || config.defaultTo,
        categoryId: elements.categoryInput?.value || ''
    };

    const formatNumber = (value) => {
        if (value === null || value === undefined || Number.isNaN(value)) {
            return '--';
        }
        return Number(value).toLocaleString('vi-VN');
    };

    const formatPercent = (value, suffix = '%') => {
        if (value === null || value === undefined || Number.isNaN(value)) {
            return '--';
        }
        return `${Number(value).toFixed(1)}${suffix}`;
    };

    const formatDateDisplay = (dateString) => {
        if (!dateString) return '--';
        const date = new Date(dateString);
        if (Number.isNaN(date.getTime())) return '--';
        return date.toLocaleDateString('vi-VN');
    };

    const formatDateInput = (date) => {
        const year = date.getFullYear();
        const month = `${date.getMonth() + 1}`.padStart(2, '0');
        const day = `${date.getDate()}`.padStart(2, '0');
        return `${year}-${month}-${day}`;
    };

    const toggleLoading = (isLoading) => {
        if (!elements.loading) return;
        elements.loading.classList.toggle('d-none', !isLoading);
    };

    const showError = (message) => {
        if (!elements.error) return;
        if (!message) {
            elements.error.classList.add('d-none');
            elements.error.textContent = '';
            return;
        }
        elements.error.textContent = message;
        elements.error.classList.remove('d-none');
    };

    const updateMeta = (data) => {
        if (!elements.meta) return;
        const fromText = formatDateDisplay(currentRange.from);
        const toText = formatDateDisplay(currentRange.to);
        const categoryText = currentRange.categoryId
            ? `· Danh mục ID: ${currentRange.categoryId}`
            : '';
        const newProducts = data?.periodStats?.newProducts;
        const growth = data?.periodStats?.growthPercentage;

        const newText = typeof newProducts === 'number'
            ? `${formatNumber(newProducts)} bài đăng mới`
            : '--';
        const growthText = typeof growth === 'number'
            ? `${growth >= 0 ? 'Tăng' : 'Giảm'} ${formatPercent(Math.abs(growth))}`
            : '--';

        elements.meta.textContent = `Giai đoạn: ${fromText} - ${toText} · ${newText} · ${growthText} ${categoryText}`.trim();
    };

    const updateOverview = (data) => {
        const total = data?.totalProducts ?? 0;
        const approved = data?.statusDistribution?.approved ?? 0;
        const pending = data?.statusDistribution?.pending ?? 0;
        const rejected = data?.statusDistribution?.rejected ?? 0;

        if (elements.totalPostsCount) elements.totalPostsCount.textContent = formatNumber(total);
        if (elements.totalPostsGrowth) {
            const growth = data?.periodStats?.growthPercentage;
            if (typeof growth === 'number') {
                elements.totalPostsGrowth.textContent =
                    `${growth >= 0 ? '▲' : '▼'} ${formatPercent(Math.abs(growth))} so với đầu kỳ`;
                elements.totalPostsGrowth.classList.toggle('text-success', growth >= 0);
                elements.totalPostsGrowth.classList.toggle('text-danger', growth < 0);
            } else {
                elements.totalPostsGrowth.textContent = '--';
            }
        }

        if (elements.approvedPostsCount) elements.approvedPostsCount.textContent = formatNumber(approved);
        if (elements.approvedPostsPercent) {
            const percent = total ? (approved / total) * 100 : 0;
            elements.approvedPostsPercent.textContent = `${formatPercent(percent)} tổng`;
        }

        if (elements.pendingPostsCount) elements.pendingPostsCount.textContent = formatNumber(pending);
        if (elements.pendingPostsPercent) {
            const percent = total ? (pending / total) * 100 : 0;
            elements.pendingPostsPercent.textContent = `${formatPercent(percent)} tổng`;
        }

        if (elements.rejectedPostsCount) elements.rejectedPostsCount.textContent = formatNumber(rejected);
        if (elements.rejectedPostsPercent) {
            const percent = total ? (rejected / total) * 100 : 0;
            elements.rejectedPostsPercent.textContent = `${formatPercent(percent)} tổng`;
        }

        if (elements.averageViewsCurrent) {
            elements.averageViewsCurrent.textContent = formatNumber(data?.averageViewsThisMonth) + ' lượt';
        }
        if (elements.averageViewsChange) {
            const change = data?.viewsChange;
            if (typeof change === 'number') {
                elements.averageViewsChange.textContent =
                    `${change >= 0 ? '▲' : '▼'} ${formatPercent(Math.abs(change))} so với tháng trước`;
                elements.averageViewsChange.classList.toggle('text-success', change >= 0);
                elements.averageViewsChange.classList.toggle('text-danger', change < 0);
            } else {
                elements.averageViewsChange.textContent = '--';
            }
        }
    };

    const buildTimelineDataset = (timeline = []) => {
        if (!Array.isArray(timeline) || !timeline.length) {
            return {
                labels: [],
                newProducts: [],
                approvedProducts: [],
                pendingProducts: [],
                rejectedProducts: [],
                totalProducts: [],
                mode: 'Ngày'
            };
        }

        const sorted = [...timeline].sort(
            (a, b) => new Date(a.period).getTime() - new Date(b.period).getTime()
        );

        if (sorted.length <= MAX_DAILY_POINTS) {
            return {
                labels: sorted.map((item) => formatDateDisplay(item.period)),
                newProducts: sorted.map((item) => item.newProducts),
                approvedProducts: sorted.map((item) => item.approvedProducts),
                pendingProducts: sorted.map((item) => item.pendingProducts),
                rejectedProducts: sorted.map((item) => item.rejectedProducts),
                totalProducts: sorted.map((item) => item.totalProducts),
                mode: 'Ngày'
            };
        }

        const aggregated = new Map();
        sorted.forEach((item) => {
            const date = new Date(item.period);
            if (Number.isNaN(date.getTime())) return;
            const key = `${date.getFullYear()}-${date.getMonth() + 1}`;
            const label = `T${date.getMonth() + 1}/${date.getFullYear()}`;
            if (!aggregated.has(key)) {
                aggregated.set(key, {
                    label,
                    newProducts: 0,
                    approvedProducts: 0,
                    pendingProducts: 0,
                    rejectedProducts: 0,
                    totalProducts: item.totalProducts
                });
            }
            const bucket = aggregated.get(key);
            bucket.newProducts += item.newProducts;
            bucket.approvedProducts += item.approvedProducts;
            bucket.pendingProducts += item.pendingProducts;
            bucket.rejectedProducts += item.rejectedProducts;
            bucket.totalProducts = item.totalProducts;
        });

        const values = Array.from(aggregated.values());
        return {
            labels: values.map((item) => item.label),
            newProducts: values.map((item) => item.newProducts),
            approvedProducts: values.map((item) => item.approvedProducts),
            pendingProducts: values.map((item) => item.pendingProducts),
            rejectedProducts: values.map((item) => item.rejectedProducts),
            totalProducts: values.map((item) => item.totalProducts),
            mode: 'Tháng'
        };
    };

    const renderChart = (key, ctx, config) => {
        if (!ctx) return;
        if (charts[key]) {
            charts[key].destroy();
        }
        charts[key] = new Chart(ctx, config);
    };

    const updateTrendChart = (timeline = []) => {
        const dataset = buildTimelineDataset(timeline);

        if (elements.timelineMode) {
            elements.timelineMode.textContent = `Theo ${dataset.mode.toLowerCase()}`;
        }

        if (elements.timelineNotes) {
            elements.timelineNotes.textContent = dataset.mode === 'Ngày'
                ? '≤ 60 bản ghi nên hiển thị theo ngày.'
                : 'Khoảng thời gian dài, dữ liệu được gộp theo tháng.';
        }

        renderChart('trend', chartContexts.trend, {
            type: 'line',
            data: {
                labels: dataset.labels,
                datasets: [
                    {
                        label: 'Bài mới',
                        data: dataset.newProducts,
                        borderColor: 'rgb(78, 115, 223)',
                        backgroundColor: 'rgba(78, 115, 223, 0.15)',
                        tension: 0.3,
                        fill: true,
                        pointRadius: 2
                    },
                    {
                        label: 'Đã duyệt',
                        data: dataset.approvedProducts,
                        borderColor: 'rgb(28, 200, 138)',
                        backgroundColor: 'rgba(28, 200, 138, 0.15)',
                        tension: 0.3,
                        fill: true,
                        pointRadius: 2
                    },
                    {
                        label: 'Chờ duyệt',
                        data: dataset.pendingProducts,
                        borderColor: 'rgb(246, 194, 62)',
                        backgroundColor: 'rgba(246, 194, 62, 0.15)',
                        tension: 0.3,
                        fill: true,
                        pointRadius: 2
                    },
                    {
                        label: 'Đã từ chối',
                        data: dataset.rejectedProducts,
                        borderColor: 'rgb(231, 74, 59)',
                        backgroundColor: 'rgba(231, 74, 59, 0.15)',
                        tension: 0.3,
                        fill: true,
                        pointRadius: 2
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                interaction: { mode: 'index', intersect: false },
                plugins: {
                    legend: { position: 'top' },
                    tooltip: {
                        callbacks: {
                            label: (context) =>
                                `${context.dataset.label}: ${formatNumber(context.parsed.y)}`
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: (value) => formatNumber(value)
                        }
                    }
                }
            }
        });
    };

    const updateStatusChart = (distribution = {}) => {
        const approved = distribution.approved ?? 0;
        const pending = distribution.pending ?? 0;
        const rejected = distribution.rejected ?? 0;

        renderChart('status', chartContexts.status, {
            type: 'doughnut',
            data: {
                labels: ['Đã duyệt', 'Chờ duyệt', 'Đã từ chối'],
                datasets: [
                    {
                        data: [approved, pending, rejected],
                        backgroundColor: [
                            'rgb(28, 200, 138)',
                            'rgb(246, 194, 62)',
                            'rgb(231, 74, 59)'
                        ],
                        borderWidth: 1
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { position: 'bottom' }
                }
            }
        });

        if (elements.statusApprovedCount) elements.statusApprovedCount.textContent = formatNumber(approved);
        if (elements.statusPendingCount) elements.statusPendingCount.textContent = formatNumber(pending);
        if (elements.statusRejectedCount) elements.statusRejectedCount.textContent = formatNumber(rejected);
    };

    const updateParentCategoryChart = (parentStats = []) => {
        const labels = parentStats.map((item) => item.categoryName);
        const counts = parentStats.map((item) => item.productCount);

        renderChart('parentCategory', chartContexts.parentCategory, {
            type: 'bar',
            data: {
                labels,
                datasets: [
                    {
                        label: 'Số bài đăng',
                        data: counts,
                        backgroundColor: 'rgba(78, 115, 223, 0.8)'
                    }
                ]
            },
            options: {
                indexAxis: 'y',
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            label: (context) =>
                                `${context.label}: ${formatNumber(context.parsed.x)}`
                        }
                    }
                },
                scales: {
                    x: {
                        beginAtZero: true,
                        ticks: {
                            callback: (value) => formatNumber(value)
                        }
                    }
                }
            }
        });

        if (elements.parentCategoryList) {
            if (!parentStats.length) {
                elements.parentCategoryList.innerHTML = '<div class="text-muted small">Chưa có dữ liệu</div>';
            } else {
                elements.parentCategoryList.innerHTML = `
                    <ul class="list-group list-group-flush">
                        ${parentStats
                            .map((item) => `
                                <li class="list-group-item d-flex justify-content-between align-items-center">
                                    <span>${item.categoryName}</span>
                                    <span><strong>${formatNumber(item.productCount)}</strong> · ${formatPercent(item.percentage)}</span>
                                </li>
                            `)
                            .join('')}
                    </ul>
                `;
            }
        }
    };

    const updateTopCategories = (categories = []) => {
        if (!elements.topCategoriesBody) return;
        if (!categories.length) {
            elements.topCategoriesBody.innerHTML = `
                <tr>
                    <td colspan="3" class="text-center text-muted">Chưa có dữ liệu</td>
                </tr>`;
            return;
        }

        elements.topCategoriesBody.innerHTML = categories
            .map((item) => `
                <tr>
                    <td>${item.categoryName}</td>
                    <td class="text-center">${item.parentCategoryName || '-'}</td>
                    <td class="text-center fw-bold">${formatNumber(item.productCount)}</td>
                </tr>
            `)
            .join('');
    };

    const updateUi = (data) => {
        updateMeta(data);
        updateOverview(data);
        updateTrendChart(data?.timeline);
        updateStatusChart(data?.statusDistribution);
        updateParentCategoryChart(data?.parentCategoryStats || []);
        updateTopCategories(data?.topCategories || []);
    };

    const buildQueryUrl = (from, to, categoryId) => {
        const url = new URL(config.summaryEndpoint, window.location.origin);
        if (from) url.searchParams.set('from', from);
        if (to) url.searchParams.set('to', to);
        if (categoryId) url.searchParams.set('categoryId', categoryId);
        return url;
    };

    const fetchSummary = async (from, to, categoryId) => {
        const response = await fetch(buildQueryUrl(from, to, categoryId), {
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        let payload = null;
        try {
            payload = await response.json();
        } catch {
            throw new Error('Không thể phân tích dữ liệu trả về.');
        }

        if (!response.ok || payload?.success === false) {
            const message =
                payload?.message ||
                payload?.title ||
                Object.values(payload?.errors || {})?.[0]?.[0];
            throw new Error(message || 'Không thể tải thống kê bài đăng.');
        }

        if (!payload?.data) {
            throw new Error('API không trả về dữ liệu thống kê.');
        }

        return payload.data;
    };

    const loadStatistics = async (from, to, categoryId) => {
        toggleLoading(true);
        showError('');
        try {
            const data = await fetchSummary(from, to, categoryId);
            latestSummary = data;
            currentRange = { from, to, categoryId };
            updateUi(data);
        } catch (error) {
            console.error(error);
            showError(error.message || 'Đã xảy ra lỗi.');
        } finally {
            toggleLoading(false);
        }
    };

    const ensureValidRange = () => {
        const startValue = elements.startInput?.value;
        const endValue = elements.endInput?.value;
        if (!startValue || !endValue) return false;
        if (startValue > endValue) {
            showError('Từ ngày không được lớn hơn đến ngày.');
            return false;
        }
        showError('');
        return true;
    };

    elements.form?.addEventListener('submit', (event) => {
        event.preventDefault();
        if (!ensureValidRange()) return;
        const from = elements.startInput.value;
        const to = elements.endInput.value;
        const categoryRaw = elements.categoryInput.value;
        const categoryId = categoryRaw ? Number(categoryRaw) : undefined;
        loadStatistics(from, to, categoryId);
    });

    elements.quickRange?.addEventListener('change', () => {
        const value = elements.quickRange.value;
        if (value === 'custom') {
            return;
        }
        const days = Number(value);
        if (Number.isNaN(days)) return;

        const endDate = new Date(elements.endInput.value || config.defaultTo);
        const startDate = new Date(endDate);
        startDate.setDate(startDate.getDate() - (days - 1));

        elements.startInput.value = formatDateInput(startDate);
        elements.form.requestSubmit();
    });

    elements.exportBtn?.addEventListener('click', () => {
        if (!latestSummary) {
            showError('Chưa có dữ liệu để xuất.');
            return;
        }

        const exportData = {
            range: currentRange,
            summary: latestSummary
        };

        const blob = new Blob([JSON.stringify(exportData, null, 2)], { type: 'application/json' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `post-statistics-${currentRange.from}-to-${currentRange.to}.json`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    });

    elements.printBtn?.addEventListener('click', () => window.print());

    if (currentRange.from && currentRange.to) {
        loadStatistics(currentRange.from, currentRange.to, currentRange.categoryId || undefined);
    }
})();


