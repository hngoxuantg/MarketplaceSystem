(() => {
    const config = window.USER_STATISTICS_CONFIG || {};
    const summaryEndpoint = config.summaryEndpoint;
    if (!summaryEndpoint) {
        console.warn('Không tìm thấy endpoint thống kê người dùng.');
        return;
    }

    const elements = {
        form: document.getElementById('userStatisticsFilter'),
        startInput: document.getElementById('startDate'),
        endInput: document.getElementById('endDate'),
        quickRange: document.getElementById('quickRange'),
        loading: document.getElementById('userStatisticsLoading'),
        error: document.getElementById('userStatisticsError'),
        meta: document.getElementById('userStatisticsMeta'),
        totalUsersCount: document.getElementById('totalUsersCount'),
        totalUsersGrowth: document.getElementById('totalUsersGrowth'),
        activeUsersCount: document.getElementById('activeUsersCount'),
        activeUsersPercent: document.getElementById('activeUsersPercent'),
        activeUsersGrowth: document.getElementById('activeUsersGrowth'),
        newUsersThisMonth: document.getElementById('newUsersThisMonth'),
        newUsersChange: document.getElementById('newUsersChange'),
        pendingUsersCount: document.getElementById('pendingUsersCount'),
        lockedUsersCount: document.getElementById('lockedUsersCount'),
        timelineMode: document.getElementById('timelineMode'),
        timelineNotes: document.getElementById('timelineNotes'),
        statusActiveCount: document.getElementById('statusActiveCount'),
        statusLockedCount: document.getElementById('statusLockedCount'),
        statusPendingCount: document.getElementById('statusPendingCount'),
        topUsersBody: document.getElementById('topActiveUsersBody'),
        topUsersTotal: document.getElementById('topActiveUsersTotal'),
        topLocationsList: document.getElementById('topLocationsList'),
        exportBtn: document.getElementById('exportUserStats'),
        printBtn: document.getElementById('printUserStats')
    };

    const chartContexts = {
        growth: document.getElementById('userGrowthChart')?.getContext('2d'),
        status: document.getElementById('statusChart')?.getContext('2d'),
        gender: document.getElementById('genderChart')?.getContext('2d'),
        age: document.getElementById('ageChart')?.getContext('2d'),
        location: document.getElementById('locationChart')?.getContext('2d')
    };

    const charts = {
        growth: null,
        status: null,
        gender: null,
        age: null,
        location: null
    };

    const MAX_DAILY_POINTS = 60;
    let latestSummary = null;
    let currentRange = {
        from: elements.startInput?.value || config.defaultFrom,
        to: elements.endInput?.value || config.defaultTo
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
        const growth = data?.periodStats?.growthPercentage;
        const newUsers = data?.periodStats?.newUsers;
        const growthText = typeof growth === 'number'
            ? `${growth >= 0 ? 'Tăng' : 'Giảm'} ${formatPercent(Math.abs(growth))}`
            : '--';
        const newUserText = typeof newUsers === 'number'
            ? `${formatNumber(newUsers)} người dùng mới`
            : '--';
        elements.meta.textContent = `Giai đoạn: ${fromText} - ${toText} · ${newUserText} · ${growthText}`;
    };

    const updateOverview = (data) => {
        const {
            totalUsersCount,
            totalUsersGrowth,
            activeUsersCount,
            activeUsersPercent,
            activeUsersGrowth,
            newUsersThisMonth,
            newUsersChange,
            pendingUsersCount,
            lockedUsersCount
        } = elements;

        if (totalUsersCount) totalUsersCount.textContent = formatNumber(data?.totalUsers);
        if (totalUsersGrowth) {
            const growth = data?.periodStats?.growthPercentage;
            totalUsersGrowth.textContent = typeof growth === 'number'
                ? `${growth >= 0 ? '▲' : '▼'} ${formatPercent(Math.abs(growth))} so với đầu kỳ`
                : '--';
            totalUsersGrowth.classList.toggle('text-success', growth >= 0);
            totalUsersGrowth.classList.toggle('text-danger', growth < 0);
        }

        if (activeUsersCount) {
            const active = data?.activeUsers;
            const percent = data?.activeUsersPercentage;
            activeUsersCount.textContent = `${formatNumber(active)}`;
            if (activeUsersPercent) {
                activeUsersPercent.textContent = typeof percent === 'number'
                    ? `${formatPercent(percent)} tổng người dùng`
                    : '--';
            }
        }

        if (activeUsersGrowth) {
            const activeGrowth = data?.activeUsersPeriodStats?.ActiveUsersGrowthPercentage ??
                data?.activeUsersPeriodStats?.activeUsersGrowthPercentage;
            const formatted = typeof activeGrowth === 'number'
                ? `${activeGrowth >= 0 ? '▲' : '▼'} ${formatPercent(Math.abs(activeGrowth))} người dùng hoạt động`
                : '--';
            activeUsersGrowth.textContent = formatted;
        }

        if (newUsersThisMonth) {
            const newUsers = data?.newUsersThisMonth ?? data?.periodStats?.newUsers;
            newUsersThisMonth.textContent = formatNumber(newUsers);
        }

        if (newUsersChange) {
            const change = data?.newUsersChange;
            newUsersChange.textContent = typeof change === 'number'
                ? `${change >= 0 ? '▲' : '▼'} ${formatPercent(Math.abs(change))} so với tháng trước`
                : '--';
            newUsersChange.classList.toggle('text-success', change >= 0);
            newUsersChange.classList.toggle('text-danger', change < 0);
        }

        if (pendingUsersCount) {
            pendingUsersCount.textContent = `${formatNumber(data?.statusDistribution?.pendingEmailConfirmation)} chờ xác thực`;
        }

        if (lockedUsersCount) {
            lockedUsersCount.textContent = `Đã khóa: ${formatNumber(data?.statusDistribution?.locked)}`;
        }
    };

    const buildTimelineDataset = (timeline = []) => {
        if (!Array.isArray(timeline) || !timeline.length) {
            return { labels: [], newUsers: [], totals: [], mode: 'Ngày' };
        }

        const sorted = [...timeline].sort(
            (a, b) => new Date(a.period).getTime() - new Date(b.period).getTime()
        );

        if (sorted.length <= MAX_DAILY_POINTS) {
            return {
                labels: sorted.map((item) => formatDateDisplay(item.period)),
                newUsers: sorted.map((item) => item.newUsers),
                totals: sorted.map((item) => item.totalUsers),
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
                aggregated.set(key, { label, newUsers: 0, totalUsers: item.totalUsers });
            }

            const entry = aggregated.get(key);
            entry.newUsers += item.newUsers;
            entry.totalUsers = item.totalUsers;
        });

        const values = Array.from(aggregated.values());
        return {
            labels: values.map((item) => item.label),
            newUsers: values.map((item) => item.newUsers),
            totals: values.map((item) => item.totalUsers),
            mode: 'Tháng'
        };
    };

    const renderChart = (key, config) => {
        if (!chartContexts[key]) {
            return;
        }

        if (charts[key]) {
            charts[key].destroy();
        }

        charts[key] = new Chart(chartContexts[key], config);
    };

    const updateTimelineChart = (timeline = []) => {
        const dataset = buildTimelineDataset(timeline);

        if (elements.timelineMode) {
            elements.timelineMode.textContent = `Theo ${dataset.mode.toLowerCase()}`;
        }

        if (elements.timelineNotes) {
            elements.timelineNotes.textContent = dataset.mode === 'Ngày'
                ? 'Dữ liệu được tổng hợp theo ngày (≤ 60 bản ghi).'
                : 'Khoảng thời gian dài, dữ liệu được gộp theo tháng.';
        }

        renderChart('growth', {
            type: 'line',
            data: {
                labels: dataset.labels,
                datasets: [
                    {
                        label: 'Người dùng mới',
                        data: dataset.newUsers,
                        borderColor: 'rgb(78, 115, 223)',
                        backgroundColor: 'rgba(78, 115, 223, 0.1)',
                        tension: 0.4,
                        fill: true,
                        pointRadius: 2
                    },
                    {
                        label: 'Tổng người dùng',
                        data: dataset.totals,
                        borderColor: 'rgb(28, 200, 138)',
                        backgroundColor: 'rgba(28, 200, 138, 0.1)',
                        tension: 0.4,
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
                            label: (context) => {
                                return `${context.dataset.label}: ${formatNumber(context.parsed.y)}`;
                            }
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
        const active = distribution.active ?? 0;
        const locked = distribution.locked ?? 0;
        const pending = distribution.pendingEmailConfirmation ?? 0;

        renderChart('status', {
            type: 'doughnut',
            data: {
                labels: ['Đang hoạt động', 'Đã khóa', 'Chờ xác thực'],
                datasets: [
                    {
                        data: [active, locked, pending],
                        backgroundColor: [
                            'rgb(28, 200, 138)',
                            'rgb(231, 74, 59)',
                            'rgb(246, 194, 62)'
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

        if (elements.statusActiveCount) elements.statusActiveCount.textContent = formatNumber(active);
        if (elements.statusLockedCount) elements.statusLockedCount.textContent = formatNumber(locked);
        if (elements.statusPendingCount) elements.statusPendingCount.textContent = formatNumber(pending);
    };

    const updateGenderChart = (gender = {}) => {
        renderChart('gender', {
            type: 'pie',
            data: {
                labels: ['Nam', 'Nữ', 'Khác'],
                datasets: [
                    {
                        data: [
                            gender.male ?? 0,
                            gender.female ?? 0,
                            gender.other ?? 0
                        ],
                        backgroundColor: [
                            'rgb(78, 115, 223)',
                            'rgb(231, 74, 59)',
                            'rgb(133, 135, 150)'
                        ]
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { position: 'bottom' } }
            }
        });
    };

    const updateAgeChart = (age = {}) => {
        renderChart('age', {
            type: 'doughnut',
            data: {
                labels: ['<18', '18-24', '25-34', '35-44', '45-54', '55-64', '65+'],
                datasets: [
                    {
                        data: [
                            age.under18 ?? 0,
                            age.from18To24 ?? 0,
                            age.from25To34 ?? 0,
                            age.from35To44 ?? 0,
                            age.from45To54 ?? 0,
                            age.from55To64 ?? 0,
                            age.above65 ?? 0
                        ],
                        backgroundColor: [
                            'rgb(133, 135, 150)',
                            'rgb(78, 115, 223)',
                            'rgb(28, 200, 138)',
                            'rgb(246, 194, 62)',
                            'rgb(231, 74, 59)',
                            'rgb(102, 16, 242)',
                            'rgb(54, 185, 204)'
                        ]
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { position: 'bottom' } }
            }
        });
    };

    const updateLocationChart = (locationDistribution = {}) => {
        const locations = locationDistribution.topLocations || [];
        const labels = locations.map((item) => item.locationName);
        const counts = locations.map((item) => item.userCount);

        renderChart('location', {
            type: 'bar',
            data: {
                labels,
                datasets: [
                    {
                        label: 'Số người dùng',
                        data: counts,
                        backgroundColor: 'rgba(78, 115, 223, 0.7)'
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                indexAxis: 'y',
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            label: (context) => `${context.label}: ${formatNumber(context.parsed.x)}`
                        }
                    }
                },
                scales: {
                    x: {
                        ticks: {
                            callback: (value) => formatNumber(value)
                        },
                        beginAtZero: true
                    }
                }
            }
        });

        if (elements.topLocationsList) {
            if (!locations.length) {
                elements.topLocationsList.innerHTML = '<div class="text-muted small">Chưa có dữ liệu</div>';
            } else {
                const otherPercent = locationDistribution.otherPercentage ?? 0;
                elements.topLocationsList.innerHTML = `
                    <ul class="list-group list-group-flush">
                        ${locations
                            .map((item) => `
                                <li class="list-group-item d-flex justify-content-between align-items-center">
                                    <span>${item.locationName}</span>
                                    <span><strong>${formatNumber(item.userCount)}</strong> · ${formatPercent(item.percentage, '%')}</span>
                                </li>
                            `)
                            .join('')}
                        <li class="list-group-item d-flex justify-content-between align-items-center text-muted">
                            <span>Khu vực khác</span>
                            <span>${formatPercent(otherPercent, '%')}</span>
                        </li>
                    </ul>
                `;
            }
        }
    };

    const updateTopUsers = (users = []) => {
        if (!elements.topUsersBody) return;
        if (!users.length) {
            elements.topUsersBody.innerHTML = `
                <tr>
                    <td colspan="2" class="text-center text-muted">Chưa có dữ liệu</td>
                </tr>`;
            return;
        }

        elements.topUsersBody.innerHTML = users
            .map((user) => {
                const avatar = user.avatar && user.avatar.startsWith('http')
                    ? user.avatar
                    : (user.avatar ? `${user.avatar}` : null);
                const displayName = user.fullName?.trim() || user.userName;
                return `
                    <tr>
                        <td>
                            <div class="d-flex align-items-center">
                                <div class="rounded-circle bg-light me-2" style="width:36px;height:36px;overflow:hidden;">
                                    ${avatar
                                        ? `<img src="${avatar}" alt="${displayName}" class="img-fluid">`
                                        : '<i class="fas fa-user text-secondary d-flex justify-content-center align-items-center h-100"></i>'}
                                </div>
                                <div>
                                    <div class="fw-semibold">${displayName}</div>
                                    <small class="text-muted">${user.userName}</small>
                                </div>
                            </div>
                        </td>
                        <td class="text-center fw-bold">${formatNumber(user.totalProductViews)}</td>
                    </tr>
                `;
            })
            .join('');

        if (elements.topUsersTotal) {
            elements.topUsersTotal.textContent = `Top ${users.length} · Theo lượt xem sản phẩm`;
        }
    };

    const updateCharts = (data) => {
        updateTimelineChart(data?.timeline);
        updateStatusChart(data?.statusDistribution);
        updateGenderChart(data?.genderDistribution);
        updateAgeChart(data?.ageDistribution);
        updateLocationChart(data?.locationDistribution);
        updateTopUsers(data?.topActiveUsers || []);
    };

    const fetchSummary = async (from, to) => {
        const url = new URL(summaryEndpoint, window.location.origin);
        if (from) url.searchParams.set('from', from);
        if (to) url.searchParams.set('to', to);

        const response = await fetch(url, {
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
            const apiMessage =
                payload?.message ||
                payload?.title ||
                Object.values(payload?.errors || {})?.[0]?.[0];
            throw new Error(apiMessage || 'Không thể tải thống kê người dùng.');
        }

        if (!payload?.data) {
            throw new Error('API không trả về dữ liệu thống kê.');
        }

        return payload.data;
    };

    const loadStatistics = async (from, to) => {
        toggleLoading(true);
        showError('');
        try {
            const data = await fetchSummary(from, to);
            latestSummary = data;
            currentRange = { from, to };
            updateMeta(data);
            updateOverview(data);
            updateCharts(data);
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
        loadStatistics(from, to);
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
        const link = document.createElement('a');
        link.href = url;
        link.download = `user-statistics-${currentRange.from}-to-${currentRange.to}.json`;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        URL.revokeObjectURL(url);
    });

    elements.printBtn?.addEventListener('click', () => window.print());

    // Initial load
    if (currentRange.from && currentRange.to) {
        loadStatistics(currentRange.from, currentRange.to);
    }
})();

