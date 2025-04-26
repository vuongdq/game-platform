import React, { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import {
  Container,
  Paper,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Button,
  Select,
  MenuItem,
  FormControl,
  Box,
  Alert,
  Tabs,
  Tab,
  TextField,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Snackbar,
  CircularProgress,
  RadioGroup,
  FormControlLabel,
  Radio,
  Switch,
  Card,
  CardContent,
  Grid,
  IconButton,
  Tooltip,
  Avatar,
  Chip,
  Divider,
  useTheme,
  alpha
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  PlayArrow as PlayIcon,
  Image as ImageIcon,
  Category as CategoryIcon,
  Person as PersonIcon,
  Games as GamesIcon
} from '@mui/icons-material';

const API_BASE_URL = 'http://localhost:5000';

const AdminDashboard = () => {
  const theme = useTheme();
  const { user } = useAuth();
  const [activeTab, setActiveTab] = useState(0);
  const [users, setUsers] = useState([]);
  const [games, setGames] = useState([]);
  const [categories, setCategories] = useState([]);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(true);
  const [openDialog, setOpenDialog] = useState(false);
  const [dialogType, setDialogType] = useState('');
  const [selectedItem, setSelectedItem] = useState(null);
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    imageUrl: '',
    gameUrl: '',
    categoryId: '',
    isActive: true,
    playCount: 0,
    lastPlayed: null,
    createdAt: new Date(),
    updatedAt: null
  });
  const [selectedImage, setSelectedImage] = useState(null);
  const [selectedGameFile, setSelectedGameFile] = useState(null);
  const [uploadProgress, setUploadProgress] = useState(0);
  const [imagePreview, setImagePreview] = useState(null);
  const [gameFileInfo, setGameFileInfo] = useState(null);
  const [imageUploadMethod, setImageUploadMethod] = useState('url');
  const [gameUploadMethod, setGameUploadMethod] = useState('url');

  const MAX_IMAGE_SIZE = 5 * 1024 * 1024; // 5MB
  const MAX_GAME_SIZE = 50 * 1024 * 1024; // 50MB
  const ALLOWED_IMAGE_TYPES = ['image/jpeg', 'image/png', 'image/gif', 'image/bmp', 'image/webp'];
  const ALLOWED_GAME_TYPES = ['application/x-shockwave-flash'];

  useEffect(() => {
    if (user?.role === 'Admin') {
      fetchData();
    }
  }, [user, activeTab]);

  const fetchData = async () => {
    try {
      setLoading(true);
      setError('');
      const [usersRes, gamesRes, categoriesRes] = await Promise.all([
        fetch('http://localhost:5000/api/admin/users', {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        }),
        fetch('http://localhost:5000/api/game', {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        }),
        fetch('http://localhost:5000/api/category', {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        })
      ]);

      if (!usersRes.ok) {
        const errorData = await usersRes.json();
        throw new Error(errorData.message || 'Failed to fetch users');
      }
      if (!gamesRes.ok) {
        const errorData = await gamesRes.json();
        throw new Error(errorData.message || 'Failed to fetch games');
      }
      if (!categoriesRes.ok) {
        const errorData = await categoriesRes.json();
        throw new Error(errorData.message || 'Failed to fetch categories');
      }

      const [usersData, gamesData, categoriesData] = await Promise.all([
        usersRes.json(),
        gamesRes.json(),
        categoriesRes.json()
      ]);

      // Log the data to check
      console.log('Users data:', usersData);
      console.log('Games data:', gamesData);
      console.log('Categories data:', categoriesData);

      // Ensure users data has required fields
      const processedUsers = usersData.map(user => ({
        id: user.id,
        username: user.username || '',
        email: user.email || '',
        role: user.role || 'User',
        isActive: user.isActive ?? true,
        createdAt: user.createdAt || new Date().toISOString(),
        updatedAt: user.updatedAt || null
      }));

      setUsers(processedUsers);
      setGames(gamesData);
      setCategories(categoriesData);
    } catch (err) {
      console.error('Error fetching data:', err);
      setError('Failed to load data: ' + (err.message || 'Unknown error'));
    } finally {
      setLoading(false);
    }
  };

  const handleTabChange = (event, newValue) => {
    setActiveTab(newValue);
  };

  const handleOpenDialog = (type, item = null) => {
    setDialogType(type);
    setSelectedItem(item);
    if (item) {
      setFormData({
        name: item.name || '',
        description: item.description || '',
        imageUrl: item.imageUrl || '',
        gameUrl: item.gameUrl || '',
        categoryId: item.categoryId || '',
        isActive: item.isActive ?? true,
        playCount: item.playCount || 0,
        lastPlayed: item.lastPlayed || null,
        createdAt: item.createdAt || new Date(),
        updatedAt: item.updatedAt || null
      });
    } else {
      setFormData({
        name: '',
        description: '',
        imageUrl: '',
        gameUrl: '',
        categoryId: '',
        isActive: true,
        playCount: 0,
        lastPlayed: null,
        createdAt: new Date(),
        updatedAt: null
      });
    }
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setSelectedItem(null);
    setFormData({
      name: '',
      description: '',
      imageUrl: '',
      gameUrl: '',
      categoryId: '',
      isActive: true,
      playCount: 0,
      lastPlayed: null,
      createdAt: new Date(),
      updatedAt: null
    });
  };

  const handleCloseSnackbar = () => {
    setError('');
    setSuccess('');
  };

  const validateFile = (file, type) => {
    if (type === 'image') {
      if (file.size > MAX_IMAGE_SIZE) {
        throw new Error(`File size must be less than ${MAX_IMAGE_SIZE / (1024 * 1024)}MB`);
      }
      if (!ALLOWED_IMAGE_TYPES.includes(file.type)) {
        throw new Error('Only JPG, PNG, GIF, BMP, and WEBP images are allowed');
      }
    } else if (type === 'game') {
      if (file.size > MAX_GAME_SIZE) {
        throw new Error(`File size must be less than ${MAX_GAME_SIZE / (1024 * 1024)}MB`);
      }
      if (!ALLOWED_GAME_TYPES.includes(file.type)) {
        throw new Error('Only SWF files are allowed');
      }
    }
  };

  const handleImageChange = (event) => {
    try {
      if (event.target.files && event.target.files[0]) {
        const file = event.target.files[0];
        validateFile(file, 'image');
        
        // Create preview
        const reader = new FileReader();
        reader.onload = (e) => {
          setImagePreview(e.target.result);
        };
        reader.readAsDataURL(file);
        
        setSelectedImage(file);
        setError('');
      }
    } catch (err) {
      setError(err.message);
      setSelectedImage(null);
      setImagePreview(null);
    }
  };

  const handleGameFileChange = (event) => {
    try {
      if (event.target.files && event.target.files[0]) {
        const file = event.target.files[0];
        validateFile(file, 'game');
        
        setGameFileInfo({
          name: file.name,
          size: (file.size / (1024 * 1024)).toFixed(2) + 'MB',
          type: file.type
        });
        
        setSelectedGameFile(file);
        setError('');
      }
    } catch (err) {
      setError(err.message);
      setSelectedGameFile(null);
      setGameFileInfo(null);
    }
  };

  const uploadFile = async (file, type) => {
    try {
      setUploadProgress(0);
      const formData = new FormData();
      formData.append('file', file);

      const endpoint = type === 'image' ? 'image' : 'game';
      const response = await fetch(`http://localhost:5000/api/upload/${endpoint}`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: formData
      });

      if (!response.ok) {
        const errorText = await response.text();
        let errorMessage = 'Upload failed';
        try {
          const errorData = JSON.parse(errorText);
          errorMessage = errorData.message || errorMessage;
        } catch (e) {
          errorMessage = errorText || errorMessage;
        }
        throw new Error(errorMessage);
      }

      const data = await response.json();
      // Ensure the path starts with a forward slash
      const path = data.path.startsWith('/') ? data.path : `/${data.path}`;
      return path;
    } catch (error) {
      console.error('Upload error:', error);
      setError('Failed to upload file: ' + error.message);
      return null;
    }
  };

  const handleSubmit = async () => {
    try {
      setError('');
      let url = '';
      let method = '';
      let body = {};

      if (dialogType === 'game') {
        url = selectedItem
          ? `http://localhost:5000/api/game/${selectedItem.id}`
          : 'http://localhost:5000/api/game';
        method = selectedItem ? 'PUT' : 'POST';

        // Upload game file if selected
        let gameUrl = formData.gameUrl;
        if (selectedGameFile) {
          gameUrl = await uploadFile(selectedGameFile, 'game');
          if (!gameUrl) return;
        }

        // Upload image if selected
        let imageUrl = formData.imageUrl;
        if (selectedImage) {
          imageUrl = await uploadFile(selectedImage, 'image');
          if (!imageUrl) return;
        }

        body = {
          name: formData.name,
          description: formData.description,
          imageUrl: imageUrl,
          gameUrl: gameUrl,
          categoryId: parseInt(formData.categoryId),
          isActive: formData.isActive
        };
      } else if (dialogType === 'category') {
        url = selectedItem
          ? `http://localhost:5000/api/category/${selectedItem.id}`
          : 'http://localhost:5000/api/category';
        method = selectedItem ? 'PUT' : 'POST';

        // Upload image if selected
        let imageUrl = formData.imageUrl;
        if (selectedImage) {
          imageUrl = await uploadFile(selectedImage, 'image');
          if (!imageUrl) return;
        }

        body = {
          name: formData.name,
          description: formData.description,
          imageUrl: imageUrl
        };
      } else if (dialogType === 'user') {
        url = selectedItem
          ? `http://localhost:5000/api/admin/users/${selectedItem.id}`
          : 'http://localhost:5000/api/admin/users';
        method = selectedItem ? 'PUT' : 'POST';

        // Prepare user data
        body = {
          id: selectedItem?.id,
          username: formData.username,
          email: formData.email,
          role: formData.role || 'User',
          isActive: formData.isActive ?? true
        };

        // Only include password for new users
        if (!selectedItem && formData.password) {
          body.password = formData.password;
        }
      }

      console.log('Sending request to:', url);
      console.log('Request body:', body);

      const response = await fetch(url, {
        method,
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(body)
      });

      if (!response.ok) {
        const errorText = await response.text();
        console.error('Error response:', errorText);
        let errorMessage = 'Operation failed';
        try {
          const errorData = JSON.parse(errorText);
          errorMessage = errorData.message || errorMessage;
        } catch (e) {
          errorMessage = errorText || errorMessage;
        }
        throw new Error(errorMessage);
      }

      const responseData = await response.json();
      console.log('Response data:', responseData);

      handleCloseDialog();
      fetchData();
      setSuccess(`${dialogType} ${selectedItem ? 'updated' : 'created'} successfully`);
    } catch (error) {
      console.error('Error in handleSubmit:', error);
      setError(error.message);
    }
  };

  const handleDelete = async (type, id) => {
    if (window.confirm(`Are you sure you want to delete this ${type}?`)) {
      try {
        setError('');
        const url = type === 'game'
          ? `http://localhost:5000/api/game/${id}`
          : type === 'category'
          ? `http://localhost:5000/api/category/${id}`
          : `http://localhost:5000/api/admin/users/${id}`;

        const response = await fetch(url, {
          method: 'DELETE',
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        });

        if (!response.ok) {
          const errorText = await response.text();
          let errorMessage = `Failed to delete ${type}`;
          try {
            const errorData = JSON.parse(errorText);
            errorMessage = errorData.message || errorMessage;
          } catch (e) {
            errorMessage = errorText || errorMessage;
          }
          throw new Error(errorMessage);
        }

        fetchData();
        setSuccess(`${type} deleted successfully`);
      } catch (err) {
        setError('Failed to delete item: ' + err.message);
      }
    }
  };

  const handleRoleChange = async (userId, newRole) => {
    try {
      setError('');
      const user = users.find(u => u.id === userId);
      if (!user) {
        throw new Error('User not found');
      }

      const response = await fetch(`http://localhost:5000/api/admin/users/${userId}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify({
          id: userId,
          username: user.username,
          email: user.email,
          role: newRole,
          isActive: user.isActive ?? true
        })
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || 'Failed to update role');
      }

      // Update local state
      setUsers(users.map(u => 
        u.id === userId ? { ...u, role: newRole } : u
      ));
      
      setSuccess('User role updated successfully');
    } catch (err) {
      console.error('Error updating role:', err);
      setError('Failed to update user role: ' + err.message);
    }
  };

  const handleDeleteUser = async (userId) => {
    if (window.confirm('Are you sure you want to delete this user?')) {
      try {
        setError('');
        const response = await fetch(`http://localhost:5000/api/admin/users/${userId}`, {
          method: 'DELETE',
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        });

        if (!response.ok) {
          const errorData = await response.json();
          throw new Error(errorData.message || 'Failed to delete user');
        }

        const data = await response.json();
        setSuccess(data.message || 'User deleted successfully');
        setUsers(users.filter(user => user.id !== userId));
      } catch (err) {
        setError('Failed to delete user: ' + err.message);
      }
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Container maxWidth="xl">
      <Box sx={{ width: '100%', mt: 4 }}>
        <Card sx={{ mb: 4, boxShadow: 3 }}>
          <CardContent>
            <Typography variant="h4" component="h1" gutterBottom>
              Admin Dashboard
            </Typography>
            <Typography variant="subtitle1" color="text.secondary">
              Manage your platform content and users
            </Typography>
          </CardContent>
        </Card>

        <Card sx={{ mb: 4, boxShadow: 3 }}>
          <CardContent>
            <Tabs 
              value={activeTab} 
              onChange={handleTabChange}
              variant="fullWidth"
              sx={{
                '& .MuiTab-root': {
                  textTransform: 'none',
                  fontWeight: 'bold',
                }
              }}
            >
              <Tab 
                icon={<PersonIcon />} 
                label="Users" 
                iconPosition="start" 
              />
              <Tab 
                icon={<GamesIcon />} 
                label="Games" 
                iconPosition="start" 
              />
              <Tab 
                icon={<CategoryIcon />} 
                label="Categories" 
                iconPosition="start" 
              />
            </Tabs>

            {error && (
              <Alert 
                severity="error" 
                sx={{ mt: 2 }}
                onClose={() => setError('')}
              >
                {error}
              </Alert>
            )}

            {success && (
              <Alert 
                severity="success" 
                sx={{ mt: 2 }}
                onClose={() => setSuccess('')}
              >
                {success}
              </Alert>
            )}

            <Box sx={{ mt: 2 }}>
              {activeTab === 0 && (
                <>
                  <Box sx={{ mb: 2, display: 'flex', justifyContent: 'flex-end' }}>
                    <Button
                      variant="contained"
                      color="primary"
                      startIcon={<AddIcon />}
                      onClick={() => handleOpenDialog('user')}
                    >
                      Add User
                    </Button>
                  </Box>
                  <TableContainer component={Paper} elevation={0}>
                    <Table>
                      <TableHead>
                        <TableRow>
                          <TableCell>Username</TableCell>
                          <TableCell>Email</TableCell>
                          <TableCell>Role</TableCell>
                          <TableCell align="right">Actions</TableCell>
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        {users.map((user) => (
                          <TableRow key={user.id} hover>
                            <TableCell>
                              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                <Avatar sx={{ bgcolor: theme.palette.primary.main }}>
                                  {user.username.charAt(0).toUpperCase()}
                                </Avatar>
                                {user.username}
                              </Box>
                            </TableCell>
                            <TableCell>{user.email}</TableCell>
                            <TableCell>
                              <FormControl size="small">
                                <Select
                                  value={user.role}
                                  onChange={(e) => handleRoleChange(user.id, e.target.value)}
                                  sx={{ minWidth: 120 }}
                                >
                                  <MenuItem value="User">User</MenuItem>
                                  <MenuItem value="Admin">Admin</MenuItem>
                                </Select>
                              </FormControl>
                            </TableCell>
                            <TableCell align="right">
                              <Tooltip title="Edit">
                                <IconButton
                                  color="primary"
                                  onClick={() => handleOpenDialog('user', user)}
                                >
                                  <EditIcon />
                                </IconButton>
                              </Tooltip>
                              <Tooltip title="Delete">
                                <IconButton
                                  color="error"
                                  onClick={() => handleDeleteUser(user.id)}
                                >
                                  <DeleteIcon />
                                </IconButton>
                              </Tooltip>
                            </TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </TableContainer>
                </>
              )}

              {activeTab === 1 && (
                <>
                  <Box sx={{ mb: 2, display: 'flex', justifyContent: 'flex-end' }}>
                    <Button
                      variant="contained"
                      color="primary"
                      startIcon={<AddIcon />}
                      onClick={() => handleOpenDialog('game')}
                    >
                      Add Game
                    </Button>
                  </Box>
                  <TableContainer component={Paper} elevation={0}>
                    <Table>
                      <TableHead>
                        <TableRow>
                          <TableCell>Name</TableCell>
                          <TableCell>Description</TableCell>
                          <TableCell>Category</TableCell>
                          <TableCell>Image</TableCell>
                          <TableCell>Game URL</TableCell>
                          <TableCell>Status</TableCell>
                          <TableCell>Play Count</TableCell>
                          <TableCell>Last Played</TableCell>
                          <TableCell>Created At</TableCell>
                          <TableCell>Updated At</TableCell>
                          <TableCell align="right">Actions</TableCell>
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        {games.map((game) => (
                          <TableRow key={game.id} hover>
                            <TableCell>
                              <Typography variant="body1" fontWeight="medium">
                                {game.name || 'No Name'}
                              </Typography>
                            </TableCell>
                            <TableCell>
                              <Typography variant="body2" noWrap sx={{ maxWidth: 200 }}>
                                {game.description || 'No Description'}
                              </Typography>
                            </TableCell>
                            <TableCell>
                              <Chip
                                label={categories.find(c => c.id === game.categoryId)?.name || 'Unknown Category'}
                                size="small"
                                color="primary"
                                variant="outlined"
                              />
                            </TableCell>
                            <TableCell>
                              {game.imageUrl ? (
                                <Avatar
                                  src={`${API_BASE_URL}${game.imageUrl}`}
                                  alt={game.name}
                                  variant="rounded"
                                  sx={{ width: 56, height: 56 }}
                                />
                              ) : (
                                <Avatar
                                  variant="rounded"
                                  sx={{ width: 56, height: 56, bgcolor: 'grey.300' }}
                                >
                                  <ImageIcon />
                                </Avatar>
                              )}
                            </TableCell>
                            <TableCell>
                              {game.gameUrl ? (
                                <Button
                                  variant="outlined"
                                  size="small"
                                  startIcon={<PlayIcon />}
                                  href={`${API_BASE_URL}${game.gameUrl}`}
                                  target="_blank"
                                  rel="noopener noreferrer"
                                >
                                  Play
                                </Button>
                              ) : (
                                <Typography variant="caption" color="text.secondary">
                                  No URL
                                </Typography>
                              )}
                            </TableCell>
                            <TableCell>
                              <Chip
                                label={game.isActive ? 'Active' : 'Inactive'}
                                color={game.isActive ? 'success' : 'default'}
                                size="small"
                              />
                            </TableCell>
                            <TableCell>{game.playCount || 0}</TableCell>
                            <TableCell>
                              {game.lastPlayed ? new Date(game.lastPlayed).toLocaleDateString() : 'Never'}
                            </TableCell>
                            <TableCell>
                              {game.createdAt ? new Date(game.createdAt).toLocaleDateString() : 'Unknown'}
                            </TableCell>
                            <TableCell>
                              {game.updatedAt ? new Date(game.updatedAt).toLocaleDateString() : 'Never'}
                            </TableCell>
                            <TableCell align="right">
                              <Tooltip title="Edit">
                                <IconButton
                                  color="primary"
                                  onClick={() => handleOpenDialog('game', game)}
                                >
                                  <EditIcon />
                                </IconButton>
                              </Tooltip>
                              <Tooltip title="Delete">
                                <IconButton
                                  color="error"
                                  onClick={() => handleDelete('game', game.id)}
                                >
                                  <DeleteIcon />
                                </IconButton>
                              </Tooltip>
                            </TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </TableContainer>
                </>
              )}

              {activeTab === 2 && (
                <>
                  <Box sx={{ mb: 2, display: 'flex', justifyContent: 'flex-end' }}>
                    <Button
                      variant="contained"
                      color="primary"
                      startIcon={<AddIcon />}
                      onClick={() => handleOpenDialog('category')}
                    >
                      Add Category
                    </Button>
                  </Box>
                  <TableContainer component={Paper} elevation={0}>
                    <Table>
                      <TableHead>
                        <TableRow>
                          <TableCell>Name</TableCell>
                          <TableCell>Description</TableCell>
                          <TableCell>Image</TableCell>
                          <TableCell align="right">Actions</TableCell>
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        {categories.map((category) => (
                          <TableRow key={category.id} hover>
                            <TableCell>{category.name}</TableCell>
                            <TableCell>
                              <Typography variant="body2" noWrap sx={{ maxWidth: 200 }}>
                                {category.description}
                              </Typography>
                            </TableCell>
                            <TableCell>
                              {category.imageUrl ? (
                                <Avatar
                                  src={`${API_BASE_URL}${category.imageUrl}`}
                                  alt={category.name}
                                  variant="rounded"
                                  sx={{ width: 56, height: 56 }}
                                />
                              ) : (
                                <Avatar
                                  variant="rounded"
                                  sx={{ width: 56, height: 56, bgcolor: 'grey.300' }}
                                >
                                  <ImageIcon />
                                </Avatar>
                              )}
                            </TableCell>
                            <TableCell align="right">
                              <Tooltip title="Edit">
                                <IconButton
                                  color="primary"
                                  onClick={() => handleOpenDialog('category', category)}
                                >
                                  <EditIcon />
                                </IconButton>
                              </Tooltip>
                              <Tooltip title="Delete">
                                <IconButton
                                  color="error"
                                  onClick={() => handleDelete('category', category.id)}
                                >
                                  <DeleteIcon />
                                </IconButton>
                              </Tooltip>
                            </TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </TableContainer>
                </>
              )}
            </Box>
          </CardContent>
        </Card>

        <Dialog 
          open={openDialog} 
          onClose={handleCloseDialog}
          maxWidth="md"
          fullWidth
        >
          <DialogTitle>
            {selectedItem ? `Edit ${dialogType}` : `Add ${dialogType}`}
          </DialogTitle>
          <DialogContent>
            {dialogType === 'game' ? (
              <Grid container spacing={2} sx={{ mt: 1 }}>
                <Grid item xs={12}>
                  <TextField
                    autoFocus
                    fullWidth
                    label="Name"
                    value={formData.name}
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                  />
                </Grid>
                <Grid item xs={12}>
                  <TextField
                    fullWidth
                    label="Description"
                    multiline
                    rows={4}
                    value={formData.description}
                    onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                  />
                </Grid>
                <Grid item xs={12}>
                  <Typography variant="subtitle1" gutterBottom>
                    Image
                  </Typography>
                  <FormControl component="fieldset" fullWidth>
                    <RadioGroup
                      row
                      value={imageUploadMethod}
                      onChange={(e) => setImageUploadMethod(e.target.value)}
                    >
                      <FormControlLabel value="url" control={<Radio />} label="URL" />
                      <FormControlLabel value="upload" control={<Radio />} label="Upload" />
                    </RadioGroup>
                    {imageUploadMethod === 'url' ? (
                      <TextField
                        fullWidth
                        label="Image URL"
                        value={formData.imageUrl}
                        onChange={(e) => setFormData({ ...formData, imageUrl: e.target.value })}
                        helperText="Enter a full URL or a path starting with /uploads/image/"
                      />
                    ) : (
                      <Box>
                        <input
                          type="file"
                          accept="image/*"
                          onChange={handleImageChange}
                          style={{ marginTop: '8px' }}
                        />
                        {imagePreview && (
                          <Box sx={{ 
                            mt: 2, 
                            display: 'flex', 
                            flexDirection: 'column', 
                            alignItems: 'center',
                            gap: 1
                          }}>
                            <Box sx={{
                              width: 200,
                              height: 200,
                              position: 'relative',
                              border: '1px solid #ddd',
                              borderRadius: '4px',
                              overflow: 'hidden',
                              display: 'flex',
                              alignItems: 'center',
                              justifyContent: 'center',
                              bgcolor: 'grey.100'
                            }}>
                              <img 
                                src={imagePreview} 
                                alt="Preview" 
                                style={{ 
                                  maxWidth: '100%',
                                  maxHeight: '100%',
                                  objectFit: 'contain'
                                }} 
                              />
                            </Box>
                            <Typography variant="caption" color="text.secondary">
                              {selectedImage?.name} ({(selectedImage?.size / (1024 * 1024)).toFixed(2)}MB)
                            </Typography>
                          </Box>
                        )}
                        {formData.imageUrl && !selectedImage && (
                          <Box sx={{ 
                            mt: 2, 
                            display: 'flex', 
                            flexDirection: 'column', 
                            alignItems: 'center',
                            gap: 1
                          }}>
                            <Box sx={{
                              width: 200,
                              height: 200,
                              position: 'relative',
                              border: '1px solid #ddd',
                              borderRadius: '4px',
                              overflow: 'hidden',
                              display: 'flex',
                              alignItems: 'center',
                              justifyContent: 'center',
                              bgcolor: 'grey.100'
                            }}>
                              <img 
                                src={`${API_BASE_URL}${formData.imageUrl}`} 
                                alt="Current" 
                                style={{ 
                                  maxWidth: '100%',
                                  maxHeight: '100%',
                                  objectFit: 'contain'
                                }} 
                              />
                            </Box>
                            <Typography variant="caption" color="text.secondary">
                              Current image
                            </Typography>
                          </Box>
                        )}
                      </Box>
                    )}
                  </FormControl>
                </Grid>
                <Grid item xs={12}>
                  <Typography variant="subtitle1" gutterBottom>
                    Game File
                  </Typography>
                  <FormControl component="fieldset" fullWidth>
                    <RadioGroup
                      row
                      value={gameUploadMethod}
                      onChange={(e) => setGameUploadMethod(e.target.value)}
                    >
                      <FormControlLabel value="url" control={<Radio />} label="URL" />
                      <FormControlLabel value="upload" control={<Radio />} label="Upload" />
                    </RadioGroup>
                    {gameUploadMethod === 'url' ? (
                      <TextField
                        fullWidth
                        label="Game URL"
                        value={formData.gameUrl}
                        onChange={(e) => setFormData({ ...formData, gameUrl: e.target.value })}
                        helperText="Enter a full URL or a path starting with /uploads/game/"
                      />
                    ) : (
                      <Box>
                        <input
                          type="file"
                          accept=".swf"
                          onChange={handleGameFileChange}
                          style={{ marginTop: '8px' }}
                        />
                        {gameFileInfo && (
                          <Box sx={{ mt: 2 }}>
                            <Typography variant="body2">
                              File: {gameFileInfo.name}
                            </Typography>
                            <Typography variant="body2">
                              Size: {gameFileInfo.size}
                            </Typography>
                            <Typography variant="body2">
                              Type: SWF Game
                            </Typography>
                          </Box>
                        )}
                        {formData.gameUrl && !selectedGameFile && (
                          <Typography variant="caption" display="block" sx={{ mt: 1 }}>
                            Current game file: {formData.gameUrl}
                          </Typography>
                        )}
                      </Box>
                    )}
                  </FormControl>
                </Grid>
                <Grid item xs={12}>
                  <FormControl fullWidth>
                    <Select
                      value={formData.categoryId}
                      onChange={(e) => setFormData({ ...formData, categoryId: e.target.value })}
                      label="Category"
                    >
                      {categories.map((category) => (
                        <MenuItem key={category.id} value={category.id}>
                          {category.name}
                        </MenuItem>
                      ))}
                    </Select>
                  </FormControl>
                </Grid>
                <Grid item xs={12}>
                  <FormControlLabel
                    control={
                      <Switch
                        checked={formData.isActive}
                        onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
                      />
                    }
                    label="Active"
                  />
                </Grid>
              </Grid>
            ) : dialogType === 'category' ? (
              <Grid container spacing={2} sx={{ mt: 1 }}>
                <Grid item xs={12}>
                  <TextField
                    autoFocus
                    fullWidth
                    label="Name"
                    value={formData.name}
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                  />
                </Grid>
                <Grid item xs={12}>
                  <TextField
                    fullWidth
                    label="Description"
                    multiline
                    rows={4}
                    value={formData.description}
                    onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                  />
                </Grid>
                <Grid item xs={12}>
                  <Typography variant="subtitle1" gutterBottom>
                    Image
                  </Typography>
                  <FormControl component="fieldset" fullWidth>
                    <RadioGroup
                      row
                      value={imageUploadMethod}
                      onChange={(e) => setImageUploadMethod(e.target.value)}
                    >
                      <FormControlLabel value="url" control={<Radio />} label="URL" />
                      <FormControlLabel value="upload" control={<Radio />} label="Upload" />
                    </RadioGroup>
                    {imageUploadMethod === 'url' ? (
                      <TextField
                        fullWidth
                        label="Image URL"
                        value={formData.imageUrl}
                        onChange={(e) => setFormData({ ...formData, imageUrl: e.target.value })}
                        helperText="Enter a full URL or a path starting with /uploads/image/"
                      />
                    ) : (
                      <Box>
                        <input
                          type="file"
                          accept="image/*"
                          onChange={handleImageChange}
                          style={{ marginTop: '8px' }}
                        />
                        {imagePreview && (
                          <Box sx={{ 
                            mt: 2, 
                            display: 'flex', 
                            flexDirection: 'column', 
                            alignItems: 'center',
                            gap: 1
                          }}>
                            <Box sx={{
                              width: 200,
                              height: 200,
                              position: 'relative',
                              border: '1px solid #ddd',
                              borderRadius: '4px',
                              overflow: 'hidden',
                              display: 'flex',
                              alignItems: 'center',
                              justifyContent: 'center',
                              bgcolor: 'grey.100'
                            }}>
                              <img 
                                src={imagePreview} 
                                alt="Preview" 
                                style={{ 
                                  maxWidth: '100%',
                                  maxHeight: '100%',
                                  objectFit: 'contain'
                                }} 
                              />
                            </Box>
                            <Typography variant="caption" color="text.secondary">
                              {selectedImage?.name} ({(selectedImage?.size / (1024 * 1024)).toFixed(2)}MB)
                            </Typography>
                          </Box>
                        )}
                        {formData.imageUrl && !selectedImage && (
                          <Box sx={{ 
                            mt: 2, 
                            display: 'flex', 
                            flexDirection: 'column', 
                            alignItems: 'center',
                            gap: 1
                          }}>
                            <Box sx={{
                              width: 200,
                              height: 200,
                              position: 'relative',
                              border: '1px solid #ddd',
                              borderRadius: '4px',
                              overflow: 'hidden',
                              display: 'flex',
                              alignItems: 'center',
                              justifyContent: 'center',
                              bgcolor: 'grey.100'
                            }}>
                              <img 
                                src={`${API_BASE_URL}${formData.imageUrl}`} 
                                alt="Current" 
                                style={{ 
                                  maxWidth: '100%',
                                  maxHeight: '100%',
                                  objectFit: 'contain'
                                }} 
                              />
                            </Box>
                            <Typography variant="caption" color="text.secondary">
                              Current image
                            </Typography>
                          </Box>
                        )}
                      </Box>
                    )}
                  </FormControl>
                </Grid>
              </Grid>
            ) : (
              <Grid container spacing={2} sx={{ mt: 1 }}>
                {!selectedItem && (
                  <Grid item xs={12}>
                    <TextField
                      autoFocus
                      fullWidth
                      label="Username"
                      value={formData.username}
                      onChange={(e) => setFormData({ ...formData, username: e.target.value })}
                    />
                  </Grid>
                )}
                <Grid item xs={12}>
                  <TextField
                    fullWidth
                    label="Email"
                    value={formData.email}
                    onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                  />
                </Grid>
                <Grid item xs={12}>
                  <TextField
                    fullWidth
                    label="Password"
                    type="password"
                    value={formData.password}
                    onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                  />
                </Grid>
                <Grid item xs={12}>
                  <FormControl fullWidth>
                    <Select
                      value={formData.role}
                      onChange={(e) => setFormData({ ...formData, role: e.target.value })}
                      label="Role"
                    >
                      <MenuItem value="User">User</MenuItem>
                      <MenuItem value="Admin">Admin</MenuItem>
                    </Select>
                  </FormControl>
                </Grid>
              </Grid>
            )}
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseDialog}>Cancel</Button>
            <Button 
              onClick={handleSubmit} 
              color="primary"
              variant="contained"
            >
              {selectedItem ? 'Update' : 'Create'}
            </Button>
          </DialogActions>
        </Dialog>
      </Box>
    </Container>
  );
};

export default AdminDashboard; 